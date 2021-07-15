using WebApiClean.Domain.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WebApiClean.Domain
{
    [ExcludeFromCodeCoverage]
    public class ServiceError : ValueObject, IServiceError
    {
        /// <inheritdoc />
        public ServiceErrorCode ErrorCode { get; }

        /// <inheritdoc />
        public string Message { get; }

        private ServiceError(ServiceErrorCode errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public static bool operator ==(ServiceError left, ServiceError right)
        {
            return EqualOperator(left, right);
        }

        public static bool operator !=(ServiceError left, ServiceError right)
        {
            return NotEqualOperator(left, right);
        }

        public new ServiceError Clone() =>
            new ServiceError(ErrorCode, Message);

        public override bool Equals(object obj) =>
            ValueObjectEquals(obj as ServiceError);

        public override int GetHashCode() =>
            base.GetHashCode();

        /// <inheritdoc />
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ErrorCode;
            yield return Message;
        }

        public static IServiceError FromFailureResponse(FailureResponse response) =>
            new ServiceError(
                (ServiceErrorCode)response.ErrorCode,
                response.Message);

        public static IServiceError UnknownError() =>
            new ServiceError(
                ServiceErrorCode.UnknownError,
                "An unhandled exception occurred; check the log for more information.");

        public static IServiceError DomainError() =>
            new ServiceError(
                ServiceErrorCode.DomainError,
                "An unforeseen domain exception occurred; check the log for more information.");

        public static IServiceError OperationCanceledError() =>
            new ServiceError(
                ServiceErrorCode.OperationCanceledError,
                "Operation was canceled; most likely client or proxy server had cut off our connection; check the log for more information.");

        // 20xx - Authentication
        public static class Authentication
        {
            public static IServiceError UnauthorizedAccess() =>
                new ServiceError(
                    ServiceErrorCode.Authentication_UnauthorizedAccess,
                    "Unauthorized access.");
        }

        // 21xx - Invalid parameters
        public static class InvalidParameters
        {
            public static IServiceError EmptyRequest() =>
                new ServiceError(
                    ServiceErrorCode.InvalidParameters_EmptyRequest,
                    "Http request body is missing or invalid.");

            public static IServiceError ValidationError(IDictionary<string, string[]> failures)
            {
                var errors = failures.Select(o => $"{o.Key}: [{string.Join(", ", o.Value)}]")
                    .Aggregate((o1, o2) => $"{o1}; {o2}");

                return new ServiceError(
                    ServiceErrorCode.InvalidParameters_ValidationError,
                    $"Validation error. {errors}");
            }
        }

        // 22xx - Internal errors
        public static class InternalError
        {
            public static IServiceError RestError(string message) =>
                new ServiceError(
                    ServiceErrorCode.InternalError_RestError,
                    message);

            public static IServiceError RestErrorInvalidFormat() =>
                new ServiceError(
                    ServiceErrorCode.InternalError_RestError,
                    "Invalid REST service response format.");
        }
    }
}
