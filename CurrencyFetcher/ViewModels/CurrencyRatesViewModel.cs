using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CurrencyFetcher.ViewModels
{
    internal class CurrencyRatesViewModel : INotifyPropertyChanged
    {
        private readonly ICurrencyService _currencyService;

        public DateTime DateFrom { get; set; } = DateTime.Now;
        public DateTime MinDateFrom { get; } = new(1996, 1, 1);
        public DateTime MaxDateFrom => DateTo;

        public DateTime DateTo { get; set; } = DateTime.Now;
        public DateTime MinDateTo => DateFrom;
        public DateTime MaxDateTo { get; } = DateTime.Now;

        public IReadOnlyList<CurrencyRate> Rates { get; set; } = Array.Empty<CurrencyRate>();

        public event PropertyChangedEventHandler PropertyChanged;

        public CurrencyRatesViewModel(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
    }
}
