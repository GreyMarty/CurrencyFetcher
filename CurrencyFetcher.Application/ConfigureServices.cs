using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services;
using CurrencyFetcher.Application.Services.Optimization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace CurrencyFetcher.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IBankApi, BankApi>();

        return services;
    }

    public static IServiceCollection AddStringPool(this IServiceCollection services)
    {
        return services.AddSingleton<IStringPool, InternalStringPool>();
    }

    public static IServiceCollection AddPhysicalStorage(this IServiceCollection services)
    {
        services.AddSingleton<IPhysicalStorage<IReadOnlyList<CurrencyRate>>, CurrencyRatesPhysicalStorage>(); 
        return services;
    }
}
