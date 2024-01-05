using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CurrencyFetcher.Application.Services
{
    public interface IBankApi
    {
        Task<Stream> GetRates(int periodicity, DateTime onDate);
    }

    public class BankApi : IBankApi
    {
        private readonly HttpClient _http;

        public BankApi(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://api.nbrb.by/exrates/");
        }

        public async Task<Stream> GetRates(int periodicity, DateTime onDate)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["periodicity"] = periodicity.ToString();
            query["ondate"] = onDate.ToString("yyyy-MM-dd");
            
            var url = $"rates?{query}";
            
            var result = await _http.GetAsync(url);
            result.EnsureSuccessStatusCode();

            return await result.Content.ReadAsStreamAsync();
        }
    }
}