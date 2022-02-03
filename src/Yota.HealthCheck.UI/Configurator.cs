using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Yota.HealthCheck.UI
{
    public static class Configurator
    {
        public static IEndpointRouteBuilder AddHealthCheck(this IEndpointRouteBuilder endpoints)
        {
            //adding endpoint of health check for the health check ui in UI format
            endpoints.MapHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });


            return endpoints;
        }

        public static IEndpointRouteBuilder AddHealthCheckUi(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            //map healthcheck ui endpoing - default is /healthchecks-ui/
            endpointRouteBuilder.MapHealthChecksUI();
            return endpointRouteBuilder;
        }
    }
}