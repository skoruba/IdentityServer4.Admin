using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Skoruba.IdentityServer4.STS.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) => {
                    logging.ClearProviders();
                    var logger = new LoggerConfiguration().ReadFrom.Configuration(hostingContext.Configuration).CreateLogger();
                    logging.AddSerilog(logger);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
