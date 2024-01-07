using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CurrencyFetcher.Application.Util;
using Telerik.Windows.Controls;

namespace CurrencyFetcher.ViewModels
{
    internal class ProgressBoxViewModel : INotifyPropertyChanged
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        private Task? _task;
        
        public float CurrentValue { get; set; }
        public float TargetValue { get; set; }
        public string PercentageText => $"{CurrentValue / (TargetValue + float.Epsilon) * 100:.#} %";

        public bool Finished { get; set; }
        public Visibility FinishedLabelVisibility => Finished ? Visibility.Visible : Visibility.Collapsed;
        
        public string CloseButtonText => Finished ? "Закрыть" : "Отмена";

        public ICommand LoadedCommand { get; }
        public ICommand UnloadedCommand { get; }
        public ICommand ClosingCommand { get; }
        public ICommand CloseCommand { get; }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? CloseRequested;

        public ProgressBoxViewModel(Func<IProgress<SimpleProgress>, CancellationToken, Task> doWork)
        {
            LoadedCommand = new DelegateCommand(_ =>
            {
                var progress = new Progress<SimpleProgress>();
                progress.ProgressChanged += (_, e) =>
                {
                    CurrentValue = e.CurrentValue;
                    TargetValue = e.TargetValue;
                    Finished = e.Finished;
                };

                _task = doWork(progress, _cancellationTokenSource.Token);
            });

            UnloadedCommand = new DelegateCommand(_ =>
            {
                _cancellationTokenSource.Dispose();
            });

            ClosingCommand = new DelegateCommand(async _ =>
            {
                if (_task is not null)
                {
                    await _task;
                }
            });
            
            CloseCommand = new DelegateCommand(async _ =>
            {
                _cancellationTokenSource.Cancel();
                CloseRequested?.Invoke();
            });
        }
    }
}
