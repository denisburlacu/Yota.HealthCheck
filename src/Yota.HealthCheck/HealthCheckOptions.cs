namespace Yota.HealthCheck
{
    public class HealthCheckOptions
    {
        public HealthCheckOptions()
        {
            // initialised by IOptions
            Endpoint = "";
        }

        public bool Enabled { get; set; }
        public string Endpoint { get; set; }
    }
}