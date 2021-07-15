using System;
using System.Threading.Tasks;

namespace WebApiClean.Application.Services.Interfaces
{
    public interface ICache
    {
        T GetOrAdd<T>(string key, TimeSpan cacheTime, Func<T> addFactory) where T : class;
        Task<T> GetOrAddAsync<T>(string key, TimeSpan cacheTime, Func<Task<T>> addFactory) where T : class;
        Task<T> GetAsync<T>(string key) where T : class;
        Task<T> SetAsync<T>(string key, TimeSpan cacheTime, Func<Task<T>> addFactory) where T : class;
        Task DeleteAsync(string key);
    }
}
