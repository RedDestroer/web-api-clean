using WebApiClean.Host.Middleware;
using Microsoft.AspNetCore.Builder;

namespace WebApiClean.Host.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder) =>
            builder.UseMiddleware<CustomExceptionHandlerMiddleware>();

        public static IApplicationBuilder UseCurrentRequestContext(this IApplicationBuilder builder) =>
            builder.UseMiddleware<CurrentRequestContextMiddleware>();
    }
}
