using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApiClean.Application.Config
{
    public static class ConfigExtensions
    {
        public static IDiagConfig AddDiagConfig(this IServiceCollection serviceCollection, Action<DiagConfigProvider> providerAction)
        {
            var provider = new DiagConfigProvider();
            providerAction?.Invoke(provider);

            var config = provider.GetInstance();
            serviceCollection.AddSingleton<IDiagConfig>(config);
            serviceCollection.AddSingleton<IPerformanceConfig>(config.Performance);

            return config;
        }
    }
}
