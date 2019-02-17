using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity;
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

            var host = BuildWebHost(args);

            // Uncomment this to seed upon startup, alternatively pass in `dotnet run /seed` to seed using CLI
            // await DbMigrationHelpers.EnsureSeedData<IdentityServerConfigurationDbContext, UserIdentity, UserIdentityRole>(host);
            if (seed)
            {
                await DbMigrationHelpers.EnsureSeedData<IdentityServerConfigurationDbContext, UserIdentity, UserIdentityRole>(host);
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseKestrel(c => c.AddServerHeader = false)
                   .UseStartup<Startup>()
                   .UseSerilog()
                   .Build();
    }
}