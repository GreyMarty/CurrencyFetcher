using System;
using System.Collections.Generic;
using CurrencyFetcher.Application.Models;

namespace CurrencyFetcher.ViewModels;

public class CurrencyRateSeriesViewModel
{
    public CurrencyRateSeriesViewModel(Type type, string title, IEnumerable<CurrencyRate> items)
    {
        Type = type;
        Name = title;
        Items = items;
    }
    
    public Type Type { get; set; }
    public string Name { get; set; }
    public IEnumerable<CurrencyRate> Items { get; set; }
}