using WebApiClean.Host.Formatters;
using System;
using System.Text.Json.Serialization;

namespace WebApiClean.Host.HealthCheck
{
    public class HealthCheckModel
    {
        public string Status { get; set; }
        public IndividualHealthCheckModel[] HealthChecks { get; set; } = Array.Empty<IndividualHealthCheckModel>();

        [JsonConverter(typeof(TimespanConverter))]
        public TimeSpan HealthCheckDuration { get; set; }
    }
}
