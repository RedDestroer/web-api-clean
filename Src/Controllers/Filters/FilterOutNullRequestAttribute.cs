using WebApiClean.Domain;
using WebApiClean.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiClean.Controllers.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FilterOutNullRequestAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var modelStateErrors =
                    from modelState in context.ModelState
                    let key = modelState.Key
                    let errors = modelState.Value.Errors
                    select new ModelStateError
                    {
                        Key = key,
                        Descriptions = errors?.Select(o => new ModelStateErrorDescription
                        {
                            ErrorMessage = o.ErrorMessage,
                            ExceptionMessage = o.Exception?.Message,
                        }).ToArray(),
                    };
                var failures = modelStateErrors.ToDictionary(
                    o => o.Key,
                    o => o.Descriptions.Select(d => d.ErrorMessage).ToArray());

                var error = ServiceError.InvalidParameters.ValidationError(failures);

                throw new ServiceErrorException("Model state is invalid.", error);
            }

            if (context.ActionArguments.ContainsKey("request"))
            {
                if (context.ActionArguments["request"] == null)
                {
                    var error = ServiceError.InvalidParameters.EmptyRequest();

                    throw new ServiceErrorException("Request is null.", error);
                }
            }

            await next();
        }

        [Serializable]
        private class ModelStateError
        {
            public string Key { get; set; }
            public ModelStateErrorDescription[] Descriptions { get; set; }
        }

        [Serializable]
        private class ModelStateErrorDescription
        {
            public string ErrorMessage { get; set; }
            public string ExceptionMessage { get; set; }
        }
    }
}
