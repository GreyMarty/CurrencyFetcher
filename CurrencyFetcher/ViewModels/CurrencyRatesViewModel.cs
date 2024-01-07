using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CurrencyFetcher.Application.Util;
using Telerik.Windows.Controls;

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

        public ICommand LoadFromApiCommand { get; }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<Func<IProgress<SimpleProgress>, CancellationToken, Task>>? ExecuteTaskRequested;

        public CurrencyRatesViewModel(ICurrencyService currencyService)
        {
            _currencyService = currencyService;

            LoadFromApiCommand = new DelegateCommand(_ =>
            {
                ExecuteTaskRequested?.Invoke(async (progress, cancellationToken) =>
                {
                    var rates = await _currencyService.GetRatesAsync(DateFrom, DateTo, 1, progress, cancellationToken);

                    if (rates is not null)
                    {
                        Rates = rates;
                    }
                });
            });
        }
    }
}
