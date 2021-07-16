// ReSharper disable InconsistentNaming

namespace WebApiClean.Domain.ServiceResult
{
    public enum ServiceErrorCode
    {
        UnknownError = 0,
        DomainError = 1,
        OperationCanceledError = 2,

        // 20xx - Authentication

        Authentication_UnauthorizedAccess = 2000,

        // 21xx - Invalid parameters

        InvalidParameters_EmptyRequest = 2100,
        InvalidParameters_ValidationError = 2101,

        // 22xx - Internal errors

        InternalError_RestError = 2200,

        // 23xx - NotFound
        // 24xx - Duplicate
        // 25xx - Data integrity violation
        // 30xx - X Service errors
    }
}
