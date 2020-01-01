using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Skoruba.IdentityServer4.STS.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     configApp.AddJsonFile($"serilog.json", optional: true);
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
                        .Enrich.WithProperty("ComponentName", hostContext.HostingEnvironment.ApplicationName);
                });
    }
}