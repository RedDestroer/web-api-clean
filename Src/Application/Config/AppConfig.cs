using System;
using System.Linq;

namespace WebApiClean.Application.Config
{
    public class AppConfig : IAppConfig
    {
        private string[] _allowedOrigins;
        private IDiagConfig _diag;

        public AppConfig()
        {
            _allowedOrigins = Array.Empty<string>();
            _diag = new DiagConfig();
        }

        public string[] AllowedOrigins
        {
            get => _allowedOrigins;
            set => _allowedOrigins = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IDiagConfig Diag
        {
            get => _diag;
            set => _diag = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool ShouldEnableCors()
        {
            if (AllowedOrigins == null)
                return false;

            if (AllowedOrigins.Length < 1)
                return false;

            return !AllowedOrigins.Any(string.IsNullOrWhiteSpace);
        }
    }
}
