using WebApiClean.Domain;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiClean.Host.Middleware
{
    public class CurrentRequestContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public CurrentRequestContextMiddleware(
            RequestDelegate next,
            ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers[Constants.Headers.CorrelationId].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(correlationId))
                correlationId = context.TraceIdentifier;

            var logger = _logger.ForContext(Constants.LogProperties.CorrelationId, correlationId);

            CurrentRequestContext.CreateNewContext();
            CurrentRequestContext.Current.Logger = logger;
            CurrentRequestContext.Current.CorrelationId = correlationId;

            return _next(context);
        }
    }
}
