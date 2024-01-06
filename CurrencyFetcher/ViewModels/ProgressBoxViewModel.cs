using System.ComponentModel;
using System.Windows;

namespace CurrencyFetcher.ViewModels
{
    internal class ProgressBoxViewModel : INotifyPropertyChanged
    {
        public float CurrentValue { get; set; }
        public float TargetValue { get; set; }
        public string PercentageText => $"{CurrentValue / (TargetValue + float.Epsilon) * 100:.#} %";

        public bool Finished { get; set; }
        public Visibility FinishedLabelVisibility => Finished ? Visibility.Visible : Visibility.Collapsed;

        public string CloseButtonText => Finished ? "Закрыть" : "Отмена";

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
