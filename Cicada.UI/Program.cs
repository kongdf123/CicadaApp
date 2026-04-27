using Cicada.Domain.Interfaces;
using Cicada.Infra.Data;
using Cicada.Infra.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Cicada.UI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var builder = Host.CreateDefaultBuilder()
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainForm>();

                    services.AddSingleton<SqliteConnectionFactory>(sp =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();
                        var connStr = config.GetConnectionString("Default");
                        return new SqliteConnectionFactory(connStr);
                    });
                    services.AddSingleton<ITelemetryRepository, TelemetryRepository>();
                    services.AddSingleton<DbInitializer>();
                });

            var host = builder.Build();

            //var dbInit = host.Services.GetRequiredService<DbInitializer>();
            //dbInit.Initialize();

            SetupGlobalExceptionHandling(host);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var mainForm = host.Services.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        static void SetupGlobalExceptionHandling(IHost host)
        {
            var logger = host.Services.GetRequiredService<ILogger>();

            Application.ThreadException += (sender, args) =>
            {
                logger.Error(args.Exception, "UI Thread Exception");
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                logger.Fatal(args.ExceptionObject as Exception, "Unhandled Exception");
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                logger.Error(args.Exception, "Unobserved Task Exception");
                args.SetObserved();
            };
        }

        static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    services.AddSingleton<MainForm>()
                );
    }
}