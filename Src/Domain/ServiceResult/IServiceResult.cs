namespace WebApiClean.Domain.ServiceResult
{
    public interface IServiceResult
    {
        bool IsSuccess { get; }

        IServiceError Error { get; }

        IServiceResult<TNew> ToFailedType<TNew>();

        void Deconstruct(out bool isSuccess, out IServiceError error);
    }

    public interface IServiceResult<TResult> : IServiceResult
    {
        TResult ResultData { get; }

        void Deconstruct(out bool isSuccess, out IServiceError error, out TResult result);
    }
}
