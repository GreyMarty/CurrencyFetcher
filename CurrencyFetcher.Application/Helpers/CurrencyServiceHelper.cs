using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyFetcher.Application.Models;
using Newtonsoft.Json;

namespace CurrencyFetcher.Application.Helpers
{
    internal static class CurrencyServiceHelper
    {
        public static async Task<IEnumerable<Currency>> DeserializeCurrenciesAsync(JsonSerializer json, HttpResponseMessage response, IDictionary<string, CurrencyName> currencyNames)
        {
            var result = new List<Currency>();
            var stream = await response.Content.ReadAsStreamAsync();
            
            using (var textReader = new StreamReader(stream))
            {
                var jsonReader = new JsonTextReader(textReader);

                foreach (var jsonCurrency in json.Deserialize<JsonCurrency[]>(jsonReader))
                {
                    if (!currencyNames.TryGetValue(jsonCurrency.Name, out var currencyName))
                    {
                        currencyName = new CurrencyName(jsonCurrency.Name);
                        currencyNames[jsonCurrency.Name] = currencyName;
                    }

                    var currency = new Currency
                    {
                        Abbreviation = jsonCurrency.Abbreviation,
                        Date = jsonCurrency.Date,
                        Name = currencyName,
                        OfficialRate = jsonCurrency.OfficialRate
                    };

                    result.Add(currency);
                }
            }
            
            stream.Dispose();

            return result;
        }
    }
}