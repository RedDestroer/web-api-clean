using AutoMapper;
using WebApiClean.Infrastructure.Mapping;
using System;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Host.Mapping
{
    public static class AutoMapperConfig
    {
        private static readonly Lazy<Profile> _autoProfile = new Lazy<Profile>(
            () => AutoMapperProfile.Initialize(
                typeof(IServiceError).Assembly, // Domain
                typeof(Domain.Services.DependencyInjection).Assembly, // Domain Services
                typeof(Domain.Services.Interfaces.Mapping.IHaveCustomMapping).Assembly, // Domain Services Interfaces
                typeof(Application.DependencyInjection).Assembly, // Application
                typeof(Application.Services.DependencyInjection).Assembly, // Application Services
                typeof(Application.Services.Interfaces.IServiceResultHandler).Assembly, // Application Services Interfaces
                typeof(Controllers.BaseController).Assembly, // Controllers
                typeof(Infrastructure.DependencyInjection).Assembly)); // Infrastructure

        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile(AutoProfile);
                });

            return config.CreateMapper();
        }

        public static Profile AutoProfile => _autoProfile.Value;
    }
}
