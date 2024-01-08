using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services;
using CurrencyFetcher.Application.Util;
using CurrencyFetcher.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace CurrencyFetcher.ViewModels;

internal class CurrencyRatesViewModel : INotifyPropertyChanged
{
    public CurrencyRatesViewModel(
        ICurrencyService currencyService,
        ISaveFileDialogService saveFileDialogService,
        IOpenFileDialogService openFileDialogService,
        IPhysicalStorage<IReadOnlyList<CurrencyRate>> currencyPhysicalStorage)
    {
        LoadFromApiCommand = new DelegateCommand(_ =>
        {
            ExecuteTaskRequested?.Invoke(async (progress, cancellationToken) =>
            {
                var rates = await currencyService.GetRatesAsync(DateFrom, DateTo, 1, progress, cancellationToken);

                if (rates is not null)
                {
                    Rates = rates;
                    ActiveFile = null;
                }
            });
        });

        var fileDialogOptions = new FileDialogOptions
        {
            Filter = "Текстовые файлы (*.txt;*.json)|*.txt;*.json|Все файлы (*.*)|*.*"
        };

        LoadFromFileCommand = new DelegateCommand(_ =>
        {
            if (openFileDialogService.ShowDialog(fileDialogOptions) != true)
            {
                return;
            }

            var path = openFileDialogService.Path!;

            ExecuteTaskRequested?.Invoke(async (progress, cancellationToken) =>
            {
                var rates = await currencyPhysicalStorage.LoadAsync(path, progress, cancellationToken);

                if (rates is not null)
                {
                    DateFrom = rates[0].Date;
                    DateTo = rates[rates.Count - 1].Date;
                    Rates = rates;
                    ActiveFile = path;
                }
            });
        });

        SaveToFileCommand = new DelegateCommand(_ =>
        {
            if (saveFileDialogService.ShowDialog(fileDialogOptions) != true)
            {
                return;
            }

            var path = saveFileDialogService.Path!;


            ExecuteTaskRequested?.Invoke(async (progress, cancellationToken) =>
            {
                await currencyPhysicalStorage.SaveAsync(Rates, saveFileDialogService.Path!, progress, cancellationToken);
                ActiveFile = path;
            });
        });

        SaveChangesCommand = new DelegateCommand(_ =>
        {
            if (ActiveFile is null)
            {
                SaveToFileCommand.Execute(null);
                return;
            }

            ExecuteTaskRequested?.Invoke(async (progress, cancellationToken) =>
            {
                await currencyPhysicalStorage.SaveAsync(Rates, ActiveFile, progress, cancellationToken);
            });
        });
    }

    public DateTime DateFrom { get; set; } = DateTime.Now;
    public DateTime MinDateFrom { get; } = new(1996, 1, 1);
    public DateTime MaxDateFrom => DateTo;

    public DateTime DateTo { get; set; } = DateTime.Now;
    public DateTime MinDateTo => DateFrom;
    public DateTime MaxDateTo { get; } = DateTime.Now;

    public IReadOnlyList<CurrencyRate> Rates { get; set; } = Array.Empty<CurrencyRate>();
    public IEnumerable<CurrencyRateSeriesViewModel> RateSeries => Rates
        .GroupBy(r => r.Abbreviation)
        .Select(g => new CurrencyRateSeriesViewModel(typeof(LineSeries), g.Key, g));

    public string? ActiveFile { get; set; }
    public bool CanSaveChanges => !string.IsNullOrEmpty(ActiveFile);

    public string Title => $"Курс валют{(string.IsNullOrEmpty(ActiveFile) ? "" : $" - {ActiveFile}")}";

    public ICommand LoadFromApiCommand { get; }
    public ICommand LoadFromFileCommand { get; }
    public ICommand SaveToFileCommand { get; set; }
    public ICommand SaveChangesCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<Func<IProgress<SimpleProgress>, CancellationToken, Task>>? ExecuteTaskRequested;
}