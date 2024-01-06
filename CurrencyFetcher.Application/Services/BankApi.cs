using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CurrencyFetcher.Application.Services
{
    public interface IBankApi
    {
        public Task<HttpResponseMessage> GetRatesAsync(int periodicity, DateTime onDate, CancellationToken cancellationToken = default);
    }

    public class BankApi : IBankApi
    {
        private readonly HttpClient _http;

        public BankApi(HttpClient http)
        {
            _http = http;
        }

        public async Task<HttpResponseMessage> GetRatesAsync(int periodicity, DateTime onDate, CancellationToken cancellationToken = default)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["periodicity"] = periodicity.ToString();
            query["ondate"] = onDate.ToString("yyyy-MM-dd");
            
            var url = $"rates?{query}";
            
            var result = await _http.GetAsync(url, cancellationToken);
            result.EnsureSuccessStatusCode();

            return result;
        }
    }
}