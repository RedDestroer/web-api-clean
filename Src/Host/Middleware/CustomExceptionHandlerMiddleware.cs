using WebApiClean.Application.Common.Exceptions;
using WebApiClean.Application.Services.Interfaces;
using WebApiClean.Domain;
using WebApiClean.Domain.Exceptions;
using WebApiClean.Domain.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Host.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceResultHandler _serviceResultHandler;
        private readonly IWebHostEnvironment _environment;

        public CustomExceptionHandlerMiddleware(
            RequestDelegate next,
            IServiceResultHandler serviceResultHandler,
            IWebHostEnvironment environment)
        {
            _next = next;
            _serviceResultHandler = serviceResultHandler;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_environment.IsDevelopment() || _environment.EnvironmentName == "Local")
            {
                var values = context.Request.Headers["Accept"];
                if (values.Any(o => o == "text/html"))
                {
                    await _next(context);

                    return;
                }
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var logger = AddExceptionDetails(CurrentRequestContext.Current.Logger, exception);
            logger.Error(exception, exception.Message);

            var serviceError = exception switch
            {
                ValidationException validationException => ServiceError.InvalidParameters.ValidationError(validationException.Failures),
                ServiceErrorException serviceErrorException => serviceErrorException.ServiceError,
                DomainException _ => ServiceError.DomainError(),
                TaskCanceledException _ => ServiceError.OperationCanceledError(),
                OperationCanceledException _ => ServiceError.OperationCanceledError(),
                _ => ServiceError.UnknownError()
            };

            var code = (int)_serviceResultHandler.Resolve(serviceError.ErrorCode);
            var response = serviceError.ToFailureResponse();
            var result = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;

            return context.Response.WriteAsync(result);
        }

        private static ILogger AddExceptionDetails(ILogger logger, Exception exception, string prefix = null)
        {
            if (exception.Data.Count > 0)
            {
                var propertyName = "Data";
                if (prefix != null)
                    propertyName = $"{prefix}.Data";

                logger = logger.ForContext(
                    propertyName,
                    exception.Data,
                    true);
            }

            if (exception.InnerException != null)
            {
                var innerPrefix = prefix == null
                    ? exception.InnerException.GetType().Name
                    : $"{prefix}.{exception.InnerException.GetType().Name}";

                logger = AddExceptionDetails(logger, exception.InnerException, innerPrefix);
            }

            return logger;
        }
    }
}
