using WebApiClean.Host.Middleware;
using Microsoft.AspNetCore.Builder;
using System;

namespace WebApiClean.Host.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }

        public static IApplicationBuilder UseCurrentRequestContext(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<CurrentRequestContextMiddleware>();
        }

        public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<ContentSecurityPolicyMiddleware>();
        }
    }
}
