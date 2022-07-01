using WebApiClean.Application.Config;
using MediatR;
using Serilog;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClean.Application.Common.Behaviours
{
    public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
    {
        private readonly ILogger _logger;
        private readonly IPerformanceConfig _performanceConfig;
        private readonly Stopwatch _timer;

        public RequestPerformanceBehaviour(ILogger logger, IPerformanceConfig performanceConfig)
        {
            _logger = logger;
            _performanceConfig = performanceConfig;

            _timer = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            if (_timer.Elapsed > _performanceConfig.RequestThreshold)
            {
                var name = typeof(TRequest).Name;

                _logger.Warning(
                    "Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                    name,
                    _timer.ElapsedMilliseconds,
                    request);
            }

            return response;
        }
    }
}
