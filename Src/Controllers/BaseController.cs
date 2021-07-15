using WebApiClean.Application.Services.Interfaces;
using WebApiClean.Controllers.ActionResults;
using WebApiClean.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiClean.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        private IMediator _mediator;
        private IServiceResultHandler _serviceResultHandler;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected IServiceResultHandler ServiceResultHandler => _serviceResultHandler ??= HttpContext.RequestServices.GetService<IServiceResultHandler>();

        protected IActionResult FailureResult(IServiceError error)
        {
            var statusCode = ServiceResultHandler.Resolve(error.ErrorCode);

            return new FailureResult(statusCode, error);
        }

        protected IActionResult OkResult<T>(T data) where T : class =>
            new OkObjectResult(data);

        protected IActionResult RawJsonResult(string jsonAsString)
            => new RawJsonResult(jsonAsString);
    }
}
