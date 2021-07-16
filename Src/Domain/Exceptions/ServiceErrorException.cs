using System;
using System.Diagnostics.CodeAnalysis;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Domain.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ServiceErrorException : DomainException
    {
        public ServiceErrorException()
            : this(ServiceResult.ServiceError.UnknownError())
        {
        }

        public ServiceErrorException(string message, IServiceError serviceError) : base(message)
        {
            ServiceError = serviceError;
            Data[nameof(ServiceError)] = serviceError;
        }

        public ServiceErrorException(IServiceError serviceError) : base(serviceError?.Message ?? ServiceResult.ServiceError.UnknownError().Message)
        {
            ServiceError = serviceError;
            Data[nameof(ServiceError)] = serviceError;
        }

        public ServiceErrorException(string message, IServiceError serviceError, Exception inner) : base(message, inner)
        {
            ServiceError = serviceError;
            Data[nameof(ServiceError)] = serviceError;
        }

        public ServiceErrorException(IServiceError serviceError, Exception inner) : base(serviceError?.Message ?? ServiceResult.ServiceError.UnknownError().Message, inner)
        {
            ServiceError = serviceError;
            Data[nameof(ServiceError)] = serviceError;
        }

        public IServiceError ServiceError { get; }
    }
}
