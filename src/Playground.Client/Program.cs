// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yota.HealthCheck;

var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostContext, configApp) => { configApp.SetBasePath(Directory.GetCurrentDirectory()); })
    .ConfigureServices(collection =>
    {
        collection.AddScoped<HealthCheckOptions>(new Func<IServiceProvider, HealthCheckOptions>(provider => new HealthCheckOptions()
        {
            Enabled = true,
            Endpoint = "http://18.130.171.109:910/health-check-provider/endpoint"
        }));
    })
    .ConfigureServices(services => services.AddHostedService<HealthStatusSender>());
builder.RunConsoleAsync().Wait();