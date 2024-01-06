using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services.Optimization;
using CurrencyFetcher.Application.Util;

namespace CurrencyFetcher.Application.Helpers
{
    internal static class CurrencyServiceHelper
    {
        public static async Task<IEnumerable<CurrencyRate>> DeserializeCurrenciesAsync(HttpResponseMessage response, IStringPool? stringPool = null)
        {
            using var stream = await response.Content.ReadAsStreamAsync();

            var currencies = JsonSerializer.Deserialize<CurrencyRate[]>(stream) ?? throw new FormatException();

            if (stringPool is null)
            {
                return currencies;
            }
            
            // Reuse strings
            foreach (var currency in currencies)
            {
                currency.Name = stringPool.GetOrAdd(currency.Name);
                currency.Abbreviation = stringPool.GetOrAdd(currency.Abbreviation);
            }

            return currencies;
        }

        public static SimpleProgress EstimateProgress(DateTime dateFrom, DateTime dateTo, int periodDays)
        {
            return new SimpleProgress(0, (dateTo - dateFrom).Days / periodDays + 2);
        }
    }
}