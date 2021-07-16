using System;
using System.Diagnostics.CodeAnalysis;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Domain.Extensions
{
    public static class ServiceErrorExtensions
    {
        public static FailureResponse ToFailureResponse([NotNull] this IServiceError error) =>
            new FailureResponse
            {
                ErrorCode = (int)error.ErrorCode,
                ErrorCodeDescription = Enum.GetName(typeof(ServiceErrorCode), error.ErrorCode),
                Message = error.Message,
            };
    }
}
