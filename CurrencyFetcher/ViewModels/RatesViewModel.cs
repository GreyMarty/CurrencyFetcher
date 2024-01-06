using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CurrencyFetcher.ViewModels
{
    internal class RatesViewModel : INotifyPropertyChanged
    {
        private readonly ICurrencyService _currencyService;

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public IReadOnlyList<CurrencyRate> Rates { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public RatesViewModel(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
    }
}
