using WebApiClean.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiClean.Application.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IServiceResultHandler, ServiceResultHandler>();

            return services;
        }
    }
}
