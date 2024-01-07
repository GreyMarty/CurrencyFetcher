using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services.Optimization;
using CurrencyFetcher.Application.Util;
using CurrencyFetcher.Application.Util.Helpers;

namespace CurrencyFetcher.Application.Services
{
    public interface ICurrencyService
    {
        public Task<IReadOnlyList<CurrencyRate>?> GetRatesAsync(DateTime dateFrom, DateTime dateTo, int periodDays = 1, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default);
        public Task SaveToFileAsync(IEnumerable<CurrencyRate> rates, string path, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<CurrencyRate>?> LoadFromFile(string path, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default);
    }

    public class CurrencyService : ICurrencyService
    {
        private const int MaxConcurrentTasks = 500;
        private static readonly DateTime MinDate = new(1996, 1, 1);

        private readonly IBankApi _api;
        private readonly IStringPool? _stringPool;

        public CurrencyService(IBankApi api, IStringPool? stringPool = null)
        {
            _api = api;
            _stringPool = stringPool;
        }

        public async Task<IReadOnlyList<CurrencyRate>?> GetRatesAsync(DateTime dateFrom, DateTime dateTo, int periodDays = 1, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (dateFrom < MinDate)
            {
                throw new ArgumentException($"{nameof(dateFrom)} must be greater or equal to 1996-01-01.");
            }

            if (dateTo > DateTime.Now)
            {
                throw new ArgumentException($"You can't see the future.");
            }

            if (dateTo < dateFrom)
            {
                throw new ArgumentException("Invalid range.");
            }

            if (periodDays <= 0)
            {
                throw new ArgumentException($"{nameof(periodDays)} must be greater or equal to 1.");
            }

            var progressValue = CurrencyHelper.EstimateProgress(dateFrom, dateTo, periodDays);
            progress?.Report(progressValue);

            var tasks = new List<Task>();
            var results = new ConcurrentBag<HttpResponseMessage>();

            using var semaphore = new SemaphoreSlim(MaxConcurrentTasks, MaxConcurrentTasks);

            for (var date = dateFrom; date <= dateTo; date = date.AddDays(periodDays))
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                var task = _api.GetRatesAsync(0, date, cancellationToken).ContinueWith(t => 
                {
                    if (!t.IsCanceled && !t.IsFaulted)
                    {
                        results.Add(t.Result);
                    }

                    semaphore.Release();
                    progressValue.CurrentValue++;
                    progress?.Report(progressValue);
                }, cancellationToken);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks.Where(t => !t.IsCanceled));

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var currencies = await CurrencyHelper.DeserializeCurrenciesManyAsync(results, _stringPool);

            progressValue.CurrentValue = progressValue.TargetValue;
            progressValue.Finished = true;
            progress?.Report(progressValue);

            return currencies;
        }

        public async Task<IReadOnlyList<CurrencyRate>?> LoadFromFile(string path, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            var progressValue = new SimpleProgress(0, 0);
            progress?.Report(progressValue);

            using var stream = File.OpenRead(path);
            var result = await CurrencyHelper.DeserializeCurrenciesAsync(stream, _stringPool);

            stream.Close();
            
            progressValue.Finished = true;
            progress?.Report(progressValue);
            
            return result;
        }
        
        public async Task SaveToFileAsync(IEnumerable<CurrencyRate> rates, string path, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            var progressValue = new SimpleProgress(0, 0);
            progress?.Report(progressValue);
            
            var directory = Path.GetDirectoryName(path);

            if (directory is not null)
            {
                Directory.CreateDirectory(directory);
            }
            
            using var stream = File.OpenWrite(path);

            try
            {
                await JsonSerializer.SerializeAsync(stream, rates, cancellationToken: cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            
            stream.Close();
            
            progressValue.Finished = true;
            progress?.Report(progressValue);
        }
    }
}