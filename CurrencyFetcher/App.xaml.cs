using System.Windows;
using CurrencyFetcher.Views;

namespace CurrencyFetcher;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private readonly MainWindow _mainWindow;

    public App()
    {
    }

    public App(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        MainWindow = _mainWindow;
        _mainWindow.Show();
    }
}