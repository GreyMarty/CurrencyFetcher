using System;

namespace CurrencyFetcher.Application.Services.Optimization
{
    public class InternalStringPool : IStringPool
    {
        public string GetOrAdd(string value)
        {
            return String.Intern(value);
        }
    }
}