using System;

namespace WebApiClean.Application.Config
{
    [Serializable]
    public class PerformanceConfig : IPerformanceConfig
    {
        public TimeSpan RequestThreshold { get; set; }
    }
}
