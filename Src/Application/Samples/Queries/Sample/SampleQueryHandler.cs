using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using WebApiClean.Common.Ambient;
using WebApiClean.Domain;

namespace WebApiClean.Application.Samples.Queries.Sample
{
    public class SampleQueryHandler : IRequestHandler<SampleQuery, string>
    {
        private readonly ILogger _logger;

        public SampleQueryHandler(ILogger logger)
        {
            _logger = logger;
        }

        public Task<string> Handle(SampleQuery request, CancellationToken cancellationToken)
        {
            var now = DateProvider.Current.Now();
            var response = $"{request.Echo}-{now}-{GuidProvider.Current.NewGuid()}";

            // The log result will be slightly different: second one won't contain 'CorrelationId'
            CurrentRequestContext.Current.Logger.Information("(1) Called at {Date}", now);
            _logger.Information("(2) Called at {Date}", now);

            // But is can be achieved like this
            _logger.ForContext(Constants.LogProperties.CorrelationId, CurrentRequestContext.Current.CorrelationId)
                .Information("(3) Called at {Date}", now);

            return Task.FromResult(response);
        }
    }
}
