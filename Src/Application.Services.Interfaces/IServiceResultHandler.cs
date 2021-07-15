using WebApiClean.Domain;
using System.Net;

namespace WebApiClean.Application.Services.Interfaces
{
    public interface IServiceResultHandler
    {
        HttpStatusCode Resolve(ServiceErrorCode errorCode);
    }
}
