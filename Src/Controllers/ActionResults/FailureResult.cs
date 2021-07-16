using WebApiClean.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Controllers.ActionResults
{
    public class FailureResult : IActionResult
    {
        public FailureResult(HttpStatusCode statusCode, IServiceError serviceError)
        {
            StatusCode = statusCode;
            ServiceError = serviceError;
        }

        public HttpStatusCode StatusCode { get; }
        public IServiceError ServiceError { get; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = ServiceError.ToFailureResponse();

            var objectResult = new ObjectResult(response)
            {
                StatusCode = (int)StatusCode
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
