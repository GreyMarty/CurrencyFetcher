using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services;
using CurrencyFetcher.Application.Util;
using CurrencyFetcher.Services;
using Telerik.Windows.Controls;

namespace CurrencyFetcher.ViewModels;

internal class CurrencyRatesViewModel : INotifyPropertyChanged
{
    public CurrencyRatesViewModel(ICurrencyService currencyService, ISaveFileDialogService saveFileDialogService,
        IOpenFileDialogService openFileDialogService)
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
                var rates = await currencyService.LoadFromFile(path, progress, cancellationToken);

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
                await currencyService.SaveToFileAsync(Rates, saveFileDialogService.Path!, progress, cancellationToken);
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
                await currencyService.SaveToFileAsync(Rates, ActiveFile, progress, cancellationToken);
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