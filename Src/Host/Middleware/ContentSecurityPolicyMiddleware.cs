using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApiClean.Host.Middleware
{
    public class ContentSecurityPolicyMiddleware
    {
        private readonly RequestDelegate _next;

        public ContentSecurityPolicyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");

            return _next(context);
        }
    }
}
