using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services.Optimization;
using Newtonsoft.Json;

namespace CurrencyFetcher.Application.Helpers
{
    internal static class CurrencyServiceHelper
    {
        public static async Task<IEnumerable<Currency>> DeserializeCurrenciesAsync(JsonSerializer json, HttpResponseMessage response, IStringPool? stringPool = null)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var textReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(textReader);

            var currencies = json.Deserialize<Currency[]>(jsonReader) ?? throw new FormatException();

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
    }
}