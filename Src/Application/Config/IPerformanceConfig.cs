using System;

namespace WebApiClean.Application.Config
{
    public interface IPerformanceConfig
    {
        public TimeSpan RequestThreshold { get; }
    }
}
