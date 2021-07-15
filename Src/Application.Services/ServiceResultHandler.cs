using WebApiClean.Application.Services.Interfaces;
using WebApiClean.Domain;
using System;
using System.Net;

namespace WebApiClean.Application.Services
{
    public class ServiceResultHandler : IServiceResultHandler
    {
        public HttpStatusCode Resolve(ServiceErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ServiceErrorCode.UnknownError:
                case ServiceErrorCode.DomainError:
                    return HttpStatusCode.InternalServerError;
                case ServiceErrorCode.OperationCanceledError:
                    return HttpStatusCode.BadRequest;

                // 20xx - Authentication
                case ServiceErrorCode.Authentication_UnauthorizedAccess:
                    return HttpStatusCode.Forbidden;

                // 21xx - Invalid parameters
                case ServiceErrorCode.InvalidParameters_EmptyRequest:
                case ServiceErrorCode.InvalidParameters_ValidationError:
                    return HttpStatusCode.BadRequest;

                // 22xx - Internal errors
                case ServiceErrorCode.InternalError_RestError:
                    return HttpStatusCode.BadRequest;

                default:
                    throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, $"Error code '{errorCode}' is not mapped to HttpStatusCode.");
            }
        }
    }
}
