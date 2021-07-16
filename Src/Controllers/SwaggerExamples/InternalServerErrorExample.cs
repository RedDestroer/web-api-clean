using WebApiClean.Domain.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Controllers.SwaggerExamples
{
    [ExcludeFromCodeCoverage]
    public class InternalServerErrorExample
        : IExamplesProvider<List<FailureResponse>>
    {
        public List<FailureResponse> GetExamples() =>
            new List<FailureResponse>
            {
                ServiceError.UnknownError().ToFailureResponse(),
                ServiceError.DomainError().ToFailureResponse(),
            };
    }
}
