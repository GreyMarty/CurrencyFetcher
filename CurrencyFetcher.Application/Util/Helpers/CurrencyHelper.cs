using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services.Optimization;

namespace CurrencyFetcher.Application.Util.Helpers;

internal static class CurrencyHelper
{
    public static async Task<IReadOnlyList<CurrencyRate>> DeserializeCurrenciesManyAsync(IEnumerable<HttpResponseMessage> responses, IStringPool? stringPool = null)
    {
        var result = new List<CurrencyRate>();
        var locker = new object();

        var tasks = responses
            .Select(r => r.Content.ReadAsStreamAsync())
            .Select(t => t.ContinueWith(async ct =>
            {
                using var stream = ct.Result;
                var rates = await DeserializeCurrenciesAsync(ct.Result, stringPool);

                lock (locker)
                {
                    result.AddRange(rates);
                }
            }));
        await Task.WhenAll(tasks);

        result.Sort((a, b) => a.Date.CompareTo(b.Date));
        return result;
    }
    
    public static async Task<IReadOnlyList<CurrencyRate>> DeserializeCurrenciesAsync(Stream stream, IStringPool? stringPool = null)
    {
        var currencies = await JsonSerializer.DeserializeAsync<CurrencyRate[]>(stream) ?? throw new FormatException();

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