using System;
using System.IO;
using Iserv.IdentityServer4.BusinessLogic.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace Skoruba.IdentityServer4.STS.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TracingExtensions.InitTraceIdRenderer();

            var logger = NLogBuilder.ConfigureNLog(LogManager.Configuration).GetCurrentClassLogger();

            try
            {
                logger.Info("Запуск сервиса");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    if (hostContext.HostingEnvironment.IsDevelopment())
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
                .ConfigureLogging(logging => logging.ClearProviders())
                .UseNLog();
    }
}