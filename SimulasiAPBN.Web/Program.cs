/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace SimulasiAPBN.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using var host = CreateHostBuilder(args).Build();
                using var scope = host.Services.CreateScope();
                if (!DataSeeder.Initialize(scope.ServiceProvider))
                {
                    return;
                }
                
                host.Run();
            }
            catch (Exception exception)
            {
                if (Log.Logger == null || Log.Logger.GetType().Name == "SilentLogger")
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();
                }

                Log.Fatal(exception, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var applicationName = typeof(Program).Assembly.GetName().Name ?? "SimulasiAPBN";
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .CaptureStartupErrors(true)
                        .ConfigureAppConfiguration(configuration =>
                        {
                            configuration.AddJsonFile("appsettings.Local.json", optional: true);
                        })
                        .UseSerilog((context, configuration) =>
                        {
                            var timestamp = DateTimeOffset.Now.ToString("yyyyMMdd-HHmmss");
                            configuration
                                .ReadFrom.Configuration(context.Configuration)
                                .WriteTo.Async(sinkConfiguration =>
                                {
                                    var logFileName = Path.Combine(Directory.GetCurrentDirectory(),
                                        "Logs", $"{applicationName}-{timestamp}.log");
                                    sinkConfiguration.File(logFileName);
                                    sinkConfiguration.Console();
                                })
                                .Enrich.FromLogContext()
                                .Enrich.WithExceptionDetails()
                                .Enrich.WithProperty("ApplicationName", applicationName)
                                .Enrich.WithProperty("Environment", context.HostingEnvironment);
#if DEBUG
                            configuration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
                        })
                        .UseSentry()
                        .UseShutdownTimeout(TimeSpan.FromSeconds(10));
                });
        }
    }
}
