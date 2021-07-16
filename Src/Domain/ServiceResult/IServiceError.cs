namespace WebApiClean.Domain.ServiceResult
{
    public interface IServiceError
    {
        ServiceErrorCode ErrorCode { get; }

        string Message { get; }
    }
}
