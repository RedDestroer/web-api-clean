using System;

namespace WebApiClean.Application.Config
{
    [Serializable]
    public class DiagConfig : IDiagConfig
    {
        private IPerformanceConfig _performance;

        public DiagConfig()
        {
            Performance = new PerformanceConfig();
        }

        public IPerformanceConfig Performance
        {
            get => _performance;
            set => _performance = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
