using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CurrencyFetcher.Application.Util;
using CurrencyFetcher.ViewModels;

namespace CurrencyFetcher.Views;

/// <summary>
///     Interaction logic for ProgressBox.xaml
/// </summary>
public partial class ProgressBox : Window
{
    private readonly ProgressBoxViewModel _viewModel;

    private ProgressBox(Func<IProgress<SimpleProgress>, CancellationToken, Task> doWork)
    {
        _viewModel = new ProgressBoxViewModel(doWork);
        DataContext = _viewModel;

        _viewModel.CloseRequested += Close;

        InitializeComponent();
    }

    public static bool? ExecuteTask(Func<IProgress<SimpleProgress>, CancellationToken, Task> doWork,
        Window? owner = null)
    {
        var dialog = new ProgressBox(doWork);
        dialog.Owner = owner;

        var result = dialog.ShowDialog();

        return result;
    }
}