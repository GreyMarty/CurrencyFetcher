﻿using System;
using System.Configuration;
using System.Net.Http;
using CurrencyFetcher.Application;
using CurrencyFetcher.Services;
using CurrencyFetcher.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyFetcher;

/// <summary>
///     Custom entry point, used to configure DI container
/// </summary>
internal static class EntryPoint
{
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddSingleton<App>()
            .AddSingleton<MainWindow>()
            .AddScoped(_ =>
            {
                var http = new HttpClient();
                http.BaseAddress = new Uri(ConfigurationManager.AppSettings["api"]);
                return http;
            })
            .AddApplicationServices()
            .AddStringPool()
            .AddScoped<ISaveFileDialogService, SaveFileDialogServiceService>()
            .AddScoped<IOpenFileDialogService, OpenFileDialogService>()
            .BuildServiceProvider();

        var app = services.GetRequiredService<App>();
        app.InitializeComponent();
        app.Run();
    }
}