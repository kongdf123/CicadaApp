using Cicada.Biz.Services;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cicada.Biz.Plugins;
using Cicada.Worker.Mqtt;
using Cicada.Worker.Opc;
using Cicada.Worker.Workers;
using Cicada.Domain.Models;
using Cicada.Domain.Interfaces;
using Cicada.Infra.Data;
using Cicada.Infra.Repositories;

namespace Cicada.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //// =========================
            //// 1. Logging (Serilog)
            //// =========================
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Console()
            //    .WriteTo.File("logs/service-.log", rollingInterval: RollingInterval.Day)
            //    .CreateLogger();

            try
            {
                var host = Host.CreateDefaultBuilder(args)
                    .UseSerilog((context, services, configuration) =>
                    {
                        configuration
                            .ReadFrom.Configuration(context.Configuration)
                            .ReadFrom.Services(services)
                            .Enrich.FromLogContext();
                    })
                    .ConfigureServices((context, services) =>
                    {
                        // =========================
                        // 2. Core Infrastructure
                        // =========================
                        services.AddSingleton<TelemetryChannel>();
                        services.AddSingleton<TelemetryConsumer>();
                        services.AddSingleton<TelemetryUiDispatcher>();

                        services.AddSingleton<SqliteConnectionFactory>(sp =>
                        {
                            var config = sp.GetRequiredService<IConfiguration>();
                            var connStr = config.GetConnectionString("Default");
                            return new SqliteConnectionFactory(connStr);
                        });
                        services.AddSingleton<ITelemetryRepository, TelemetryRepository>();
                        services.AddSingleton<DbInitializer>();

                        // =========================
                        // 3. Pipeline Engine
                        // =========================
                        services.AddSingleton<PipelineEngine>();

                        // Modules（顺序 = 执行顺序）
                        services.AddSingleton<IPipelineModule, DatabaseModule>();
                        services.AddSingleton<IPipelineModule, AlarmModule>();
                        services.AddSingleton<IPipelineModule, UiModule>();
                        services.AddSingleton<IPipelineModule, InfluxModule>();
                        services.AddSingleton<IPipelineModule, AnomalyModule>();

                        // =========================
                        // 4. External Services
                        // =========================
                        services.AddSingleton<OpcUaClientService>();
                        services.AddSingleton<MqttTelemetryService>();
                        services.AddSingleton<MqttPublisher>();
                        services.AddSingleton<InfluxService>();

                        // =========================
                        // 5. Worker (Background Engine)
                        // =========================
                        services.AddHostedService<DataWorker>();

                        services.AddSingleton<AlarmEventBus>();
                        services.AddSingleton<IPipelineModule, AlarmModule>();
                    })
                    .Build();

                // =========================
                // 6. Global Exception Handling
                // =========================
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    Log.Fatal(e.ExceptionObject as Exception, "Unhandled Exception");
                };

                TaskScheduler.UnobservedTaskException += (s, e) =>
                {
                    Log.Error(e.Exception, "Unobserved Task Exception");
                    e.SetObserved();
                };

                // =========================
                // 7. Run
                // =========================
                await host.RunAsync();

                Log.Information("🚀 Starting Cicada Service...");

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Service terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}