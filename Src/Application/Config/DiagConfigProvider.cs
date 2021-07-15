using Microsoft.Extensions.Configuration;

namespace WebApiClean.Application.Config
{
    public class DiagConfigProvider
    {
        private readonly DiagConfig _config;

        public DiagConfigProvider()
        {
            _config = new DiagConfig();
        }

        public DiagConfigProvider WithConfiguration(IConfiguration configuration)
        {
            configuration.GetSection("Diag").Bind(_config);

            return this;
        }

        public DiagConfig GetInstance() => _config;
    }
}
