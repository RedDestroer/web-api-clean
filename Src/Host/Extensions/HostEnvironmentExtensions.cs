using Microsoft.Extensions.Hosting;
using System;

namespace WebApiClean.Host.Extensions
{
    public static class HostEnvironmentExtensions
    {
        public static bool IsLocal(this IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
                throw new ArgumentNullException(nameof(hostEnvironment));

            return hostEnvironment.IsEnvironment("Local");
        }

        public static bool IsDevelopmentOrLocal(this IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
                throw new ArgumentNullException(nameof(hostEnvironment));

            return hostEnvironment.IsDevelopment() || hostEnvironment.IsLocal();
        }
    }
}
