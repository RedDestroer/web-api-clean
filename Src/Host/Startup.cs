using WebApiClean.Application;
using WebApiClean.Application.Services;
using WebApiClean.Controllers;
using WebApiClean.Controllers.Filters;
using WebApiClean.Domain.Services;
using WebApiClean.Host.Extensions;
using WebApiClean.Host.Formatters;
using WebApiClean.Host.HealthCheck;
using WebApiClean.Infrastructure;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiClean.Application.Config;
using WebApiClean.Domain;

namespace WebApiClean.Host
{
    public class Startup
    {
        private IServiceCollection _services;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            AppConfig = new AppConfigProvider()
                .WithConfiguration(configuration)
                .GetInstance();
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public IAppConfig AppConfig { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDomainServices();
            services.AddApplication(Configuration);
            services.AddApplicationServices();
            services.AddInfrastructure(Configuration, Environment);
            services.AddAutoMapper();
            services.AddSerilogLogger();
            services.AddRetryEnabledHttpClient();
            services.AddHttpContextAccessor();

            if (AppConfig.ShouldEnableCors())
                services.EnableCors(AppConfig.AllowedOrigins);

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

            services.AddFluentValidation(
                options =>
                {
                    options.RegisterValidatorsFromAssemblyContaining<Application.Common.Exceptions.ValidationException>();
                    options.DisableDataAnnotationsValidation = true;
                    options.ImplicitlyValidateRootCollectionElements = true;
                });

            services.AddSwagger();
            services.AddFluentValidationRulesToSwagger();

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

            if (Environment.IsDevelopmentOrLocal())
            {
                app.UseDeveloperExceptionPage();
                app.UseRegisteredServicesPage(_services);
            }

            app.UseCustomExceptionHandler();

            if (Environment.IsDevelopmentOrLocal() || Environment.IsStaging())
                app.EnableSwagger();

            app.UseContentSecurityPolicy();
            app.UseHttpsRedirection();
            app.UseRouting();

            if (AppConfig.ShouldEnableCors())
                app.UseCors(Constants.CorsProfiles.AllowOrigin);

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

        private static Task HealthResponseWriter(HttpContext context, HealthReport report)
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

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
