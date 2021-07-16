using System.Net;
using WebApiClean.Domain.ServiceResult;

namespace WebApiClean.Application.Services.Interfaces
{
    public interface IServiceResultHandler
    {
        HttpStatusCode Resolve(ServiceErrorCode errorCode);
    }
}
