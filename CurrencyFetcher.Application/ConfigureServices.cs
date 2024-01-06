using CurrencyFetcher.Application.Services;
using CurrencyFetcher.Application.Services.Optimization;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyFetcher.Application
{
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
            return services.AddScoped<IStringPool, InternalStringPool>();
        }
    }
}
