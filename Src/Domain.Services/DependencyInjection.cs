using WebApiClean.Domain.Services.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiClean.Domain.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddSingleton<DomainCustomMapping>();

            return services;
        }
    }
}
