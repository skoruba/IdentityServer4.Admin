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
    [DependsOn(typeof(MigrateAdminLogDbContext))]
    public class MigrateAdminAuditLogDbContext : MigrateAndSeedBase
    {
        public MigrateAdminAuditLogDbContext(ILogger<MigrateAdminAuditLogDbContext> logger) : base(logger)
        {
        }

        public override Task Migrate(IServiceCollection services)
        {
            return MigrateDbContext<AdminAuditLogDbContext>(services.BuildServiceProvider());
        }

        public override Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            _logger.LogInformation("Seeded {SourceContext}, no data to seed.");
            return Task.CompletedTask;
        }
    }

}
