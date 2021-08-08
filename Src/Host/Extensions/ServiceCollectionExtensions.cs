using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Linq;
using WebApiClean.Controllers.SwaggerExamples;
using WebApiClean.Domain;
using WebApiClean.Host.Mapping;

namespace WebApiClean.Host.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var mapper = AutoMapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        public static void AddSerilogLogger(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton(Log.Logger);
        }

        public static void AddRetryEnabledHttpClient(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient(Constants.HttpClients.RetryEnabled)
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(retryPolicy);
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Title = "API",
                            Version = "v1"
                        });
                    options.EnableAnnotations();
                    options.ExampleFilters();
                    options.OperationFilter<AddHeaderOperationFilter>(
                        "X-Correlation-Id",
                        "Correlation Id for the request",
                        false);
                    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                    string basePath = AppContext.BaseDirectory;

                    void IncludeIfExists(string fileName)
                    {
                        string filePath = Path.Combine(basePath, fileName);
                        if (File.Exists(filePath))
                            options.IncludeXmlComments(filePath, true);
                    }

                    IncludeIfExists("WebApiClean.Domain.xml");
                    IncludeIfExists("WebApiClean.Domain.Services.xml");
                    IncludeIfExists("WebApiClean.Domain.Services.Interfaces.xml");
                    IncludeIfExists("WebApiClean.Infrastructure.Interfaces.xml");
                    IncludeIfExists("WebApiClean.Application.xml");
                    IncludeIfExists("WebApiClean.Application.Services.xml");
                    IncludeIfExists("WebApiClean.Application.Services.Interfaces.xml");
                    IncludeIfExists("WebApiClean.Controllers.xml");
                    IncludeIfExists("WebApiClean.Infrastructure.xml");
                    IncludeIfExists("WebApiClean.Host.xml");
                });
            services.AddSwaggerExamplesFromAssemblies(typeof(InternalServerErrorExample).Assembly);
        }

        public static void EnableCors(this IServiceCollection services, string[] allowedOrigins)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (allowedOrigins == null)
                throw new ArgumentNullException(nameof(allowedOrigins));

            bool IsOriginAllowed(string origin)
            {
                var originUri = new UriBuilder(origin);

                return allowedOrigins.Any(allowedOrigin => allowedOrigin == originUri.Host);
            }

            services.AddCors(
                options =>
                {
                    options.AddPolicy(Constants.CorsProfiles.AllowOrigin,
                        config => config
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(IsOriginAllowed));
                });
        }
    }
}
