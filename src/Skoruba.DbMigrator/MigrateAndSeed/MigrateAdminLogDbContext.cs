using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Skoruba.DbMigrator.Abstractions;
using Skoruba.DbMigrator.Abstractions.Dependency;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using System.Threading.Tasks;

namespace Skoruba.DbMigrator.MigrateAndSeed
{
    [DependsOn(typeof(MigrateIdentityServerConfigurationDbContext))]
    public class MigrateAdminLogDbContext : MigrateAndSeedBase
    {
        public MigrateAdminLogDbContext(ILogger<MigrateAdminLogDbContext> logger) : base(logger)
        {
        }

        public override Task Migrate(IServiceCollection services)
        {
            return MigrateDbContext<AdminLogDbContext>(services.BuildServiceProvider());
        }

        public override Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            _logger.LogInformation("Seeded {SourceContext}, no data to seed.");
            return Task.CompletedTask;
        }
    }
}
