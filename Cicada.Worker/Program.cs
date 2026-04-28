using Cicada.Biz.Services;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cicada.Biz.Plugins;

namespace Cicada.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddSingleton<TelemetryChannel>();
                    services.AddSingleton<PluginEngine>();
                    services.AddSingleton<TelemetryConsumer>();
                    services.AddSingleton<TelemetryProducer>();
                })
                .Build()
                .Run();
        }
    }
}