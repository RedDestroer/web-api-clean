using WebApiClean.Application.Models.Sample;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace WebApiClean.Controllers.SwaggerExamples.Sample
{
    [ExcludeFromCodeCoverage]
    public class GetAllSampleRequestExample
        : IExamplesProvider<GetAllSampleRequest>
    {
        public GetAllSampleRequest GetExamples() =>
            new GetAllSampleRequest
            {
                Data = new DataModel
                {
                    Value = "sample value"
                }
            };
    }
}
