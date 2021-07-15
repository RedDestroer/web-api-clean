using WebApiClean.Application.Services.Interfaces;
using WebApiClean.Domain;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace WebApiClean.Application.Services
{
    public class Cache : ICache
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ILogger _logger;

        public Cache(ICacheProvider cacheProvider, ILogger logger)
        {
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        public T GetOrAdd<T>(string key, TimeSpan cacheTime, Func<T> addFactory) where T : class
        {
            var result = AsyncHelper.RunSync(
                async () =>
                    await GetOrAddAsync(
                            key,
                            cacheTime,
                            async () => await Task.FromResult(addFactory()).ConfigureAwait(false))
                        .ConfigureAwait(false));

            return result;
        }

        public async Task<T> GetOrAddAsync<T>(string key, TimeSpan cacheTime, Func<Task<T>> addFactory) where T : class
        {
            var isCalled = new IsCalled();
            var value = await _cacheProvider.GetOrAddAsync(
                    key,
                    cacheTime,
                    async () => await AddFactoryInnerAsync(addFactory, isCalled).ConfigureAwait(false))
                .ConfigureAwait(false);

            _logger.Debug(
                nameof(Cache),
                isCalled.Value
                    ? "Cache updated for key '{Key}'."
                    : "Cache hit for key '{Key}'.", key);

            return value;
        }

        public Task<T> GetAsync<T>(string key) where T : class =>
            _cacheProvider.GetAsync<T>(key);

        public async Task<T> SetAsync<T>(string key, TimeSpan cacheTime, Func<Task<T>> addFactory) where T : class
        {
            var isCalled = new IsCalled();
            var value = await _cacheProvider.SetAsync(
                    key,
                    cacheTime,
                    async () => await AddFactoryInnerAsync(addFactory, isCalled).ConfigureAwait(false))
                .ConfigureAwait(false);

            _logger.Debug(
                nameof(Cache),
                isCalled.Value
                    ? "Cache updated for key '{Key}'."
                    : "Cache hit for key '{Key}'.", key);

            return value;
        }

        public async Task DeleteAsync(string key)
        {
            await _cacheProvider.DeleteAsync(key)
                .ConfigureAwait(false);

            _logger.Debug(
                nameof(Cache),
                "Cache deleted for key '{Key}'.", key);
        }

        [ExcludeFromCodeCoverage]
        private static Task<T> AddFactoryInnerAsync<T>(Func<Task<T>> addFactory, IsCalled isCalled)
        {
            isCalled.Value = true;

            return addFactory();
        }

        private class IsCalled
        {
            public bool Value { get; set; }
        }
    }
}
