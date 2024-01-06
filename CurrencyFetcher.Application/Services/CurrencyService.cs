using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CurrencyFetcher.Application.Helpers;
using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services.Optimization;

namespace CurrencyFetcher.Application.Services
{
    public interface ICurrencyService
    {
        public Task<IReadOnlyList<CurrencyRate>> GetRatesAsync(DateTime dateFrom, DateTime dateTo, int periodDays = 1, CancellationToken cancellationToken = default);
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

        public async Task<IReadOnlyList<CurrencyRate>> GetRatesAsync(DateTime dateFrom, DateTime dateTo, int periodDays = 1, CancellationToken cancellationToken = default)
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

            var tasks = new List<Task<HttpResponseMessage>>();
            var continueTasks = new List<Task>();
            var semaphore = new SemaphoreSlim(MaxConcurrentTasks, MaxConcurrentTasks);

            for (var date = dateFrom; date <= dateTo; date = date.AddDays(periodDays))
            {
                await semaphore.WaitAsync(cancellationToken);

                var task = _api.GetRatesAsync(0, date);
                var continueTask = task.ContinueWith(_ => semaphore.Release(), cancellationToken);

                tasks.Add(task);
                continueTasks.Add(continueTask);
            }

            await Task.WhenAll(tasks);
            await Task.WhenAll(continueTasks);

            var currencies = new List<CurrencyRate>();

            foreach (var task in tasks)
            {
                currencies.AddRange(await CurrencyServiceHelper.DeserializeCurrenciesAsync(task.Result, _stringPool));
            }

            return currencies;
        }
    }
}