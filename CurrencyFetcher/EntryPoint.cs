using Microsoft.Extensions.DependencyInjection;
using System;

namespace CurrencyFetcher
{
    internal static class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<App>()
                .AddSingleton<MainWindow>()
                .BuildServiceProvider();

            var app = services.GetRequiredService<App>();
            app.Run();
        }
    }
}
