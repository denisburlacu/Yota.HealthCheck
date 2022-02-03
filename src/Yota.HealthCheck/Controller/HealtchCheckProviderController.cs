using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Yota.HealtchCheck.Controller
{
    [Route("health-check-provider")]
    public abstract class HealthCheckProviderController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        protected HealthCheckProviderController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [HttpGet("{consumerName}")]
        public ActionResult Get(string consumerName)
        {
            var isInitialised = _memoryCache.Get<bool>("health-initialised");

            if (!isInitialised)
            {
                Thread.Sleep(TimeSpan.FromSeconds(15));
                _memoryCache.Set("health-initialised", true);
            }

            // Report report;
            if (_memoryCache.TryGetValue("consumer-" + consumerName, out object data))
            {
                return Content(data.ToString(), "application/json");
            }


            if (_memoryCache.TryGetValue("consumer-" + consumerName, out object data2))
            {
                return Content(data2.ToString(), "application/json");
            }

            return StatusCode(404);
        }

        [HttpPost("{consumerName}")]
        public OkResult Set(string consumerName, [FromBody] object data)
        {
            _memoryCache.Set("consumer-" + consumerName, data, TimeSpan.FromSeconds(30));
            return Ok();
        }
    }
}