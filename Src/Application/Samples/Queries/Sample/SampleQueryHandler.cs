using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClean.Application.Samples.Queries.Sample
{
    public class SampleQueryHandler : IRequestHandler<SampleQuery, string>
    {
        public Task<string> Handle(SampleQuery request, CancellationToken cancellationToken)
        {
            var response = $"{request.Echo}-{request.Echo}";

            return Task.FromResult(response);
        }
    }
}
