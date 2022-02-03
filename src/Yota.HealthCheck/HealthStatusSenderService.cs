#nullable enable
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using HealthChecks.UI.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Yota.HealthCheck
{
    public class HealthStatusSender : IHostedService, IDisposable
    {
        private readonly IOptions<HealthCheckOptions> _options;
        private readonly HealthCheckService _healthCheckService;
        private Timer? _timer;
        private readonly RestClient _client;
        private readonly ILogger<HealthStatusSender> _logger;

        public HealthStatusSender(IOptions<HealthCheckOptions> options, HealthCheckService healthCheckService,
            ILogger<HealthStatusSender> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = new RestClient();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop health status sender service");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void Run()
        {
            if (!_options.Value.Enabled)
                return;

            _logger.LogInformation("Starting health status sender");

            _timer = new Timer(Start, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15));
        }

        private static readonly Lazy<JsonSerializerOptions> Options = new(
            () =>
            {
                JsonSerializerOptions serializerOptions = new()
                {
                    AllowTrailingCommas = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true
                };

                serializerOptions.Converters.Add(new JsonStringEnumConverter());
                serializerOptions.Converters.Add(new TimeSpanConverter());

                return serializerOptions;
            });

        private async void Start(object? state)
        {
            _logger.LogDebug("Starting health status sender execution");

            try
            {
                var request = new RestRequest(_options.Value.Endpoint, Method.POST);

                HealthReport report = await _healthCheckService.CheckHealthAsync();

                UIHealthReport from = UIHealthReport.CreateFrom(report);

                await using MemoryStream responseStream = new();

                await JsonSerializer.SerializeAsync(responseStream, @from, Options.Value);
                request.AddJsonBody(Encoding.ASCII.GetString(responseStream.ToArray()), "application/json");
                await _client.ExecuteAsync(request, Method.POST);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send health status details");
            }
        }
    }
}