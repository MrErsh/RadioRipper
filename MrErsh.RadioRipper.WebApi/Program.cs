using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using MrErsh.RadioRipper.WebApi;
using Serilog;
using System;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var domain = AppDomain.CurrentDomain;
            var env = new HostingEnvironment
            {
                EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                ApplicationName = domain.FriendlyName,
                ContentRootPath = domain.BaseDirectory,
            };

            var config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            try
            {
#if DEBUG
                //Log.Information("Debug test message");
                //Log.Debug("Debug test message");
                //Log.Warning("Warning test message");
                //Log.Error("Error test message");
                //Log.Fatal("Fatal test message");
#endif
                Log.Information("Starting RadioRipper web api.");
                var hostBuilder = CreateHostBuilder(args);
                var host = hostBuilder.Build();
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Radio ripper web api has terminated.");
                Console.WriteLine(ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(confLog => confLog.AddSerilog())
                .UseSerilog((context, configLogger) =>
                {
                    configLogger
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.WithProperty("AppName", "RadioRipper WebAPI")
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
