namespace WebApiClean.Application.Config
{
    public interface IAppConfig
    {
        public string[] AllowedOrigins { get; }
        public IDiagConfig Diag { get; }

        public bool ShouldEnableCors();
    }
}
