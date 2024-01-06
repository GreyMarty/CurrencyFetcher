namespace CurrencyFetcher.Application.Services.Optimization
{
    public interface IStringPool
    {
        public string GetOrAdd(string value);
    }
}