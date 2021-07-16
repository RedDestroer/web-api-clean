using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WebApiClean.Infrastructure.Interfaces;

namespace WebApiClean.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class NoCache : ICache
    {
        public T GetOrAdd<T>(string key, TimeSpan cacheTime, Func<T> addFactory) where T : class =>
            addFactory();

        public Task<T> GetOrAddAsync<T>(string key, TimeSpan cacheTime, Func<Task<T>> addFactory) where T : class =>
            addFactory();

        public Task<T> GetAsync<T>(string key) where T : class =>
            Task.FromResult<T>(default);

        public Task<T> SetAsync<T>(string key, TimeSpan cacheTime, Func<Task<T>> addFactory) where T : class =>
            addFactory();

        public Task DeleteAsync(string key) =>
            Task.CompletedTask;
    }
}
