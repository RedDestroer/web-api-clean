using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApiClean.Application.Config
{
    public static class ConfigExtensions
    {
        public static IAppConfig AddAppConfig(this IServiceCollection serviceCollection, Action<AppConfigProvider> providerAction)
        {
            var provider = new AppConfigProvider();
            providerAction?.Invoke(provider);

            var config = provider.GetInstance();
            serviceCollection.AddSingleton<IAppConfig>(config);
            serviceCollection.AddSingleton<IDiagConfig>(config.Diag);
            serviceCollection.AddSingleton<IPerformanceConfig>(config.Diag.Performance);

            return config;
        }
    }
}
