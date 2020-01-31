using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Skoruba.DbMigrator.Abstractions
{
    public abstract class MigrateAndSeedBase : IMigrateAndSeed
    {
        protected readonly ILogger _logger;

        public MigrateAndSeedBase(ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task Migrate(IServiceCollection services);
        public abstract Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot);
        protected async Task MigrateDbContext<TDbContext>(ServiceProvider serviceProvider)
            where TDbContext : DbContext
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<TDbContext>())
                {
                    await context.Database.MigrateAsync();
                    _logger.LogInformation("Migrated {SourceContext}");
                }
            }
        }
    }
}
