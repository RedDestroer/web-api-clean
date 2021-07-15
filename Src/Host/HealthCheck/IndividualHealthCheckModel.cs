namespace WebApiClean.Host.HealthCheck
{
    public class IndividualHealthCheckModel
    {
        public string Status { get; set; }
        public string Component { get; set; }
        public string Description { get; set; }
    }
}
