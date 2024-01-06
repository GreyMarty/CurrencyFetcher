using CurrencyFetcher.Application.Services;
using CurrencyFetcher.ViewModels;
using System.Windows;

namespace CurrencyFetcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CurrencyRatesViewModel _viewModel;

        public MainWindow(ICurrencyService currencyService)
        {
            _viewModel = new CurrencyRatesViewModel(currencyService);
            DataContext = _viewModel;

            InitializeComponent();
        }
    }
}
