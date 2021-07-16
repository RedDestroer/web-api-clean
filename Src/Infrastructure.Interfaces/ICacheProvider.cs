using System;
using System.Threading.Tasks;

namespace WebApiClean.Infrastructure.Interfaces
{
    public interface ICacheProvider
    {
        Task<T> GetAsync<T>(string key) where T : class;
        Task<T> GetOrAddAsync<T>(string key, TimeSpan cacheTime, Func<Task<T>> addFactory) where T : class;
        Task<T> SetAsync<T>(string key, TimeSpan cacheTime, Func<Task<T>> addFactory) where T : class;
        Task DeleteAsync(string key);
    }
}
