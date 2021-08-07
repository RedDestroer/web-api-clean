using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Web;
using WebApiClean.Common.Extensions;

namespace WebApiClean.Host.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void EnableSwagger(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        public static void UseRegisteredServicesPage(this IApplicationBuilder app, IServiceCollection services)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

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
                        foreach (var service in services)
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
}
