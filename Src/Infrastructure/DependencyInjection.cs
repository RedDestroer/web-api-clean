using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApiClean.Infrastructure.Interfaces;

namespace WebApiClean.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            //services.AddSingleton<ICacheProvider, >();
            services.AddSingleton<ICache, Cache>(); // NoCache

            return services;
        }
    }
}
