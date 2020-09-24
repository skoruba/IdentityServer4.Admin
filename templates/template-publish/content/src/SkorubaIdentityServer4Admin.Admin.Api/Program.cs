using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using SkorubaIdentityServer4Admin.Shared.Helpers;

namespace SkorubaIdentityServer4Admin.Admin.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            try
            {
                DockerHelpers.ApplyDockerConfiguration(configuration);

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration GetConfiguration(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true);

            if (isDevelopment)
            {
                configurationBuilder.AddUserSecrets<Startup>();
            }

            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder.Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     configApp.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);

                     var env = hostContext.HostingEnvironment;

                     configApp.AddJsonFile($"serilog.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                     if (env.IsDevelopment())
                     {
                         configApp.AddUserSecrets<Startup>();
                     }

                     configApp.AddEnvironmentVariables();
                     configApp.AddCommandLine(args);
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((hostContext, loggerConfig) =>
                {
                    loggerConfig
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
                });
    }
}





