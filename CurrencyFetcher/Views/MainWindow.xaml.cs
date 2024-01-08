using System.Collections.Generic;
using System.Windows;
using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services;
using CurrencyFetcher.Services;
using CurrencyFetcher.ViewModels;

namespace CurrencyFetcher.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CurrencyRatesViewModel _viewModel;

    public MainWindow(
        ICurrencyService currencyService,
        ISaveFileDialogService saveFileDialogService,
        IOpenFileDialogService openFileDialogService,
        IPhysicalStorage<IReadOnlyList<CurrencyRate>> currencyPhysicalStorage)
    {
        _viewModel = new CurrencyRatesViewModel(currencyService, saveFileDialogService, openFileDialogService, currencyPhysicalStorage);
        DataContext = _viewModel;

        _viewModel.ExecuteTaskRequested += w => ProgressBox.ExecuteTask(w, this);

        InitializeComponent();
    }
}