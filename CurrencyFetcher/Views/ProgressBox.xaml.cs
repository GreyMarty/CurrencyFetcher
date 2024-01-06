using CurrencyFetcher.Application.Util;
using CurrencyFetcher.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CurrencyFetcher.Views
{
    /// <summary>
    /// Interaction logic for ProgressBox.xaml
    /// </summary>
    public partial class ProgressBox : Window
    {
        private readonly ProgressBoxViewModel _viewModel;

        private readonly Func<IProgress<SimpleProgress>, CancellationToken, Task> _doWork;

        private Task _task;
        private CancellationTokenSource _cancellationTokenSource;

        private ProgressBox(Func<IProgress<SimpleProgress>, CancellationToken, Task> doWork)
        {
            _doWork = doWork;

            _cancellationTokenSource = new CancellationTokenSource();

            _viewModel = new ProgressBoxViewModel();
            DataContext = _viewModel;
            
            InitializeComponent();
        }

        public static bool? ExecuteTask(Func<IProgress<SimpleProgress>, CancellationToken, Task> doWork)
        {
            var dialog = new ProgressBox(doWork);
            return dialog.ShowDialog();
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();

            await _task;

            DialogResult = _viewModel.Finished;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var progress = new Progress<SimpleProgress>();
            progress.ProgressChanged += (_, e) =>
            {
                _viewModel.CurrentValue = e.CurrentValue;
                _viewModel.TargetValue = e.TargetValue;
                _viewModel.Finished = e.Finished;
            };

            _task = _doWork(progress, _cancellationTokenSource.Token);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Dispose();
        }
    }
}
