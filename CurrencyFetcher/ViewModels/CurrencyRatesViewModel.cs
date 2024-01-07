using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CurrencyFetcher.Application.Util;
using CurrencyFetcher.Services;
using Telerik.Windows.Controls;

namespace CurrencyFetcher.ViewModels
{
    internal class CurrencyRatesViewModel : INotifyPropertyChanged
    {
        public DateTime DateFrom { get; set; } = DateTime.Now;
        public DateTime MinDateFrom { get; } = new(1996, 1, 1);
        public DateTime MaxDateFrom => DateTo;

        public DateTime DateTo { get; set; } = DateTime.Now;
        public DateTime MinDateTo => DateFrom;
        public DateTime MaxDateTo { get; } = DateTime.Now;

        public IReadOnlyList<CurrencyRate> Rates { get; set; } = Array.Empty<CurrencyRate>();

        public ICommand LoadFromApiCommand { get; }
        public ICommand SaveToFileCommand { get; set; }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<Func<IProgress<SimpleProgress>, CancellationToken, Task>>? ExecuteTaskRequested;

        public CurrencyRatesViewModel(ICurrencyService currencyService, ISaveFileDialogService saveFileDialogService)
        {
            LoadFromApiCommand = new DelegateCommand(_ =>
            {
                ExecuteTaskRequested?.Invoke(async (progress, cancellationToken) =>
                {
                    var rates = await currencyService.GetRatesAsync(DateFrom, DateTo, 1, progress, cancellationToken);

                    if (rates is not null)
                    {
                        Rates = rates;
                    }
                });
            });

            SaveToFileCommand = new DelegateCommand(_ =>
            {
                var options = new SaveFileDialogOptions
                {
                    Filter = "Текстовые файлы (*.txt;*.json)|*.txt;*.json|Все файлы (*.*)|*.*"
                };
                
                if (saveFileDialogService.ShowDialog(options) != true)
                {
                    return;
                }

                var path = saveFileDialogService.Path;
                ExecuteTaskRequested?.Invoke((progress, cancellationToken) =>
                    currencyService.SaveToFileAsync(Rates, path!, progress, cancellationToken)
                );
            });
        }
    }
}
