using WebApiClean.Application.Models.Sample;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace WebApiClean.Controllers.SwaggerExamples.Sample
{
    [ExcludeFromCodeCoverage]
    public class GetAllOkExample
        : IExamplesProvider<GetAllSampleResponse>
    {
        public GetAllSampleResponse GetExamples() =>
            new GetAllSampleResponse
            {
                ResponseData = "response data"
            };
    }
}
