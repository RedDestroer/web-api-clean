using WebApiClean.Domain;
using WebApiClean.Domain.Extensions;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
