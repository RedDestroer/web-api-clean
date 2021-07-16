using WebApiClean.Application;
using WebApiClean.Application.Services;
using WebApiClean.Controllers;
using WebApiClean.Controllers.Filters;
using WebApiClean.Controllers.SwaggerExamples;
using WebApiClean.Domain;
using WebApiClean.Domain.Services;
using WebApiClean.Host.Extensions;
using WebApiClean.Host.Formatters;
using WebApiClean.Host.HealthCheck;
using WebApiClean.Host.Mapping;
using WebApiClean.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using WebApiClean.Common.Extensions;

namespace WebApiClean.Host
{
    public class Startup
    {
        private IServiceCollection _services;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDomainServices();
            services.AddApplication(Configuration);
            services.AddApplicationServices();
            services.AddInfrastructure(Configuration, Environment);

            AddAutoMapper(services);
            AddSerilogLogger(services);
            AddRetryEnabledHttpClient(services);
            AddSwagger(services);
            AddControllers(services);

            services.AddFluentValidation(
                config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<Application.Common.Exceptions.ValidationException>();
                    config.DisableDataAnnotationsValidation = true;
                    config.ImplicitlyValidateRootCollectionElements = true;
                });

            services.Configure<ApiBehaviorOptions>(
                options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            services.AddHealthChecks()
                .AddCheck<CustomHealthCheck>("Custom health check");

            _services = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCurrentRequestContext();

            if (Environment.IsDevelopment() || Environment.EnvironmentName == "Local")
            {
                app.UseDeveloperExceptionPage();
                RegisteredServicesPage(app);
            }

            app.UseCustomExceptionHandler();

            if (Environment.IsDevelopment() || Environment.EnvironmentName == "Local" || Environment.IsStaging())
                EnableSwagger(app);

            app.Use(ContentSecurityPolicyMiddleware);
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseHealthChecks(
                "/health",
                new HealthCheckOptions
                {
                    AllowCachingResponses = false,
                    ResponseWriter = HealthResponseWriter
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapControllers();
            });
        }

        private static async Task HealthResponseWriter(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            var response = new HealthCheckModel
            {
                Status = report.Status.ToString(),
                HealthChecks = report.Entries.Select(
                    o => new IndividualHealthCheckModel
                    {
                        Component = o.Key,
                        Status = o.Value.Status.ToString(),
                        Description = o.Value.Description
                    }).ToArray(),
                HealthCheckDuration = report.TotalDuration
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static Task ContentSecurityPolicyMiddleware(HttpContext context, Func<Task> next)
        {
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");

            return next();
        }

        private static void EnableSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            var mapper = AutoMapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static void AddSerilogLogger(IServiceCollection services)
            => services.AddSingleton(Log.Logger);

        private static void AddRetryEnabledHttpClient(IServiceCollection services)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient(Constants.HttpClients.RetryEnabled)
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(retryPolicy);
        }

        private static void AddSwagger(IServiceCollection services)
        {
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

        private static void AddControllers(IServiceCollection services) =>
            services.AddControllers(
                    options =>
                    {
                        options.AllowEmptyInputInBodyModelBinding = true;
                        options.InputFormatters.Insert(0, new RawRequestBodyFormatter());
                        options.Filters.Add(new FilterOutNullRequestAttribute());
                    })
                .ConfigureApiBehaviorOptions(
                    options =>
                    {
                        options.SuppressConsumesConstraintForFormFileParameters = true;
                        options.SuppressInferBindingSourcesForParameters = true;
                        options.SuppressModelStateInvalidFilter = true;
                        options.SuppressMapClientErrors = true;
                    })
                .AddApplicationPart(typeof(SampleController).Assembly);

        private void RegisteredServicesPage(IApplicationBuilder app) =>
            app.Map(
                "/services",
                builder => builder.Run(
                    async context =>
                    {
                        var sb = new StringBuilder();
                        sb.Append("<h1>Registered Services</h1>");
                        sb.Append("<table><thead>");
                        sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                        sb.Append("</thead><tbody>");
                        foreach (var service in _services)
                        {
                            sb.Append("<tr>");
                            sb.Append($"<td>{HttpUtility.HtmlEncode(service.ServiceType.GetFriendlyName())}</td>");
                            sb.Append($"<td>{HttpUtility.HtmlEncode(service.Lifetime)}</td>");
                            sb.Append($"<td>{HttpUtility.HtmlEncode(service.ImplementationType?.GetFriendlyName())}</td>");
                            sb.Append("</tr>");
                        }

                        sb.Append("</tbody></table>");
                        await context.Response.WriteAsync(sb.ToString());
                    }));
    }
}
