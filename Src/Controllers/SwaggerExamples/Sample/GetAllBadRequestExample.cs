using WebApiClean.Domain.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Controllers.SwaggerExamples.Sample
{
    [ExcludeFromCodeCoverage]
    public class GetAllBadRequestExample
        : IExamplesProvider<List<FailureResponse>>
    {
        public List<FailureResponse> GetExamples()
        {
            var failures = new Dictionary<string, string[]>
            {
                { "Failure", new[] { "Reason1", "Reason2" } }
            };

            return new List<FailureResponse>
            {
                ServiceError.InvalidParameters.EmptyRequest().ToFailureResponse(),
                ServiceError.InvalidParameters.ValidationError(failures).ToFailureResponse(),

                ServiceError.OperationCanceledError().ToFailureResponse(),
            };
        }
    }
}
