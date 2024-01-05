using System;

namespace CurrencyFetcher.Application.Models
{
    public class CurrencyName
    {
        public string Value { get; internal set; }

        public CurrencyName(string value)
        {
            Value = value;
        }

        public static implicit operator String(CurrencyName currencyName) => currencyName.Value;
        public static implicit operator CurrencyName(string value) => new CurrencyName(value);
    }
}