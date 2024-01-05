using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CurrencyFetcher.Application.Helpers;
using CurrencyFetcher.Application.Models;
using Newtonsoft.Json;

namespace CurrencyFetcher.Application.Services
{
    public class CurrencyService
    {
        private const int MaxConcurrentTasks = 500;
        private static readonly DateTime MinDate = new DateTime(1996, 1, 1);

        private readonly IBankApi _api;
        private readonly JsonSerializer _json;
        
        public CurrencyService(IBankApi api, JsonSerializer json)
        {
            _api = api;
            _json = json;
        }

        public async Task<IReadOnlyList<Currency>> GetCurrenciesAsync(DateTime dateFrom, DateTime dateTo, int periodDays = 1, CancellationToken cancellationToken = default)
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
            
            var currencies = new List<Currency>();
            var currencyNames = new ConcurrentDictionary<string, CurrencyName>();

            foreach (var task in tasks)
            {
                currencies.AddRange(await CurrencyServiceHelper.DeserializeCurrenciesAsync(_json, task.Result, currencyNames));
            }
            
            return currencies;
        }
    }
}