namespace WebApiClean.Domain
{
    public interface IServiceError
    {
        ServiceErrorCode ErrorCode { get; }

        string Message { get; }
    }
}
