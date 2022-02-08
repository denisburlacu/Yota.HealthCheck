using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Yota.HealthCheck.UI.Controller;

namespace Playground.Server.Controllers
{
    [Route("health-check-provider")]
    public class HealthCheckProviderControllerApi : HealthCheckProviderController
    {
        public HealthCheckProviderControllerApi(IMemoryCache memoryCache) : base(memoryCache)
        {
        }
    }
}