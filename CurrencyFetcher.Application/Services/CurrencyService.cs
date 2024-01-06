using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CurrencyFetcher.Application.Helpers;
using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services.Optimization;
using CurrencyFetcher.Application.Util;

namespace CurrencyFetcher.Application.Services
{
    public interface ICurrencyService
    {
        public Task<IReadOnlyList<CurrencyRate>> GetRatesAsync(DateTime dateFrom, DateTime dateTo, int periodDays = 1, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default);
    }

    public class CurrencyService : ICurrencyService
    {
        private const int MaxConcurrentTasks = 500;
        private static readonly DateTime MinDate = new(1996, 1, 1);

        private readonly IBankApi _api;
        private readonly IStringPool? _stringPool;

        public CurrencyService(IBankApi api, IStringPool? stringPool)
        {
            _api = api;
            _stringPool = stringPool;
        }

        public async Task<IReadOnlyList<CurrencyRate>> GetRatesAsync(DateTime dateFrom, DateTime dateTo, int periodDays = 1, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default)
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

            var progressValue = CurrencyServiceHelper.EstimateProgress(dateFrom, dateTo, periodDays);
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
                return Array.Empty<CurrencyRate>();
            }

            var currencies = new List<CurrencyRate>();

            foreach (var result in results)
            {
                currencies.AddRange(await CurrencyServiceHelper.DeserializeCurrenciesAsync(result, _stringPool));
            }

            progressValue.CurrentValue = progressValue.TargetValue;
            progressValue.Finished = true;
            progress?.Report(progressValue);

            currencies.Sort((a, b) => a.Date.CompareTo(b.Date));

            return currencies;
        }
    }
}