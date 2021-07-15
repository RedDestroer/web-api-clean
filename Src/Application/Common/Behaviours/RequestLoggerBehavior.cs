using MediatR.Pipeline;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClean.Application.Common.Behaviours
{
    public class RequestLoggerBehavior<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;

        public RequestLoggerBehavior(ILogger logger)
        {
            _logger = logger;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;

            _logger.Information(
                "Request: {Name} {@Request}",
                name,
                request);

            return Task.CompletedTask;
        }
    }
}
