using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Helpers;

namespace Skoruba.IdentityServer4.Admin
{
    public class Program
    {
        private const string SeedArgs = "/seed";

        public static async Task Main(string[] args)
        {
            var seed = args.Any(x => x == SeedArgs);
            if (seed) args = args.Except(new[] { SeedArgs }).ToArray();

            var host = CreateHostBuilder(args).Build();

            // Uncomment this to seed upon startup, alternatively pass in `dotnet run /seed` to seed using CLI
            // await DbMigrationHelpers.EnsureSeedData<IdentityServerConfigurationDbContext, AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext, UserIdentity, UserIdentityRole>(host);
            if (seed)
            {
                await DbMigrationHelpers.EnsureSeedData<IdentityServerConfigurationDbContext, AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext, UserIdentity, UserIdentityRole>(host);
            }

            host.Run();
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