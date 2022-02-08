using HealthChecks.UI.Configuration;

namespace Yota.HealthCheck
{
    public static class HealthCheckExtensions
    {
        public static void AddCheckEndpoint(this Settings opt, string apiUrl, string serviceName)
        {
            opt.AddHealthCheckEndpoint(serviceName, UrlHelper.Combine(apiUrl, "health-check-provider", serviceName));
        }
    }
}