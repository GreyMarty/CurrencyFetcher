namespace CurrencyFetcher.Application.Services.Optimization
{
    public interface IStringPool
    {
        string GetOrAdd(string value);
    }
}