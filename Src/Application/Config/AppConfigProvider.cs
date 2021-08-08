using Microsoft.Extensions.Configuration;
using System;

namespace WebApiClean.Application.Config
{
    public class AppConfigProvider
    {
        private readonly AppConfig _config;

        public AppConfigProvider()
        {
            _config = new AppConfig();
        }

        public AppConfigProvider WithConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _config.AllowedOrigins = (configuration.GetValue<string>("AllowedOrigins") ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries);

            configuration.GetSection("Diag").Bind(_config.Diag);

            return this;
        }

        public AppConfig GetInstance() => _config;
    }
}
