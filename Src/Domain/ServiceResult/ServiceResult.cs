using WebApiClean.Domain.Exceptions;

namespace WebApiClean.Domain.ServiceResult
{
    public static class ServiceResult
    {
        public static IServiceResult Ok()
            => new ServiceResultInternal(true, null);

        public static IServiceResult<TResult> Ok<TResult>(TResult result)
            => new ServiceResultInternal<TResult>(true, null, result);

        public static IServiceResult<ResponseWithWarnings> OkWithoutWarnings()
            => new ServiceResultInternal<ResponseWithWarnings>(true, null, new ResponseWithWarnings());

        public static IServiceResult Failed(IServiceError error)
            => new ServiceResultInternal(false, error);

        public static IServiceResult<TResult> Failed<TResult>(IServiceError error)
            => new ServiceResultInternal<TResult>(false, error, default);

        private sealed class ServiceResultInternal<TResult> : ServiceResultInternal, IServiceResult<TResult>
        {
            private readonly TResult _resultData;
            public TResult ResultData => IsSuccess ? _resultData : ThrowException();

            /// <inheritdoc />
            public void Deconstruct(out bool isSuccess, out IServiceError error, out TResult result)
            {
                isSuccess = IsSuccess;
                error = Error;
                result = IsSuccess
                    ? ResultData
                    : default;
            }

            public ServiceResultInternal(bool isSuccess, IServiceError error, TResult result)
                : base(isSuccess, error)
            {
                _resultData = result;
            }

            private static TResult ThrowException()
                => throw new DomainException("Can not access ResultData if IsSuccess is false.");
        }

        private class ServiceResultInternal : IServiceResult
        {
            public bool IsSuccess { get; }
            public IServiceError Error { get; }

            public ServiceResultInternal(bool isSuccess, IServiceError error)
            {
                IsSuccess = isSuccess;
                Error = error;
            }

            public IServiceResult<TNew> ToFailedType<TNew>()
                => Failed<TNew>(Error);

            /// <inheritdoc />
            public void Deconstruct(out bool isSuccess, out IServiceError error)
            {
                isSuccess = IsSuccess;
                error = Error;
            }
        }
    }
}
