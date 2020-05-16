using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skoruba.DbMigrator.Abstractions;
using Skoruba.DbMigrator.Abstractions.Dependency;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.EFCacheStore;
using Skoruba.MultiTenant.Stores;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Finbuckle
{
    [DependsOn("Skoruba.DbMigrator.MigrateAdminIdentityDbContext")]
    public class MigrateTenantDbContext : MigrateAndSeedBase
    {
        private readonly MultiTenantConfiguration _multiTenantConfiguration;

        public MigrateTenantDbContext(ILogger<MigrateTenantDbContext> logger, MultiTenantConfiguration multiTenantConfiguration) : base(logger)
        {
            _multiTenantConfiguration = multiTenantConfiguration;
        }

        public override async Task Migrate(IServiceCollection services)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_multiTenantConfiguration.MultiTenantEnabled)
            {
                await MigrateDbContext<EFCoreStoreDbContext>(services.BuildServiceProvider());
            }
            else
            {
                _logger.LogDebug("IsMultiTenant = false.  Skipped migration {SourceContext}.");
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        public override async Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_multiTenantConfiguration.MultiTenantEnabled)
            {
                await DoSeed<EFCoreStoreDbContext, TenantEntity>(services, configurationRoot);
            }
            else
            {
                _logger.LogDebug("IsMultiTenant = false.  Skipped seeding {SourceContext}.");
            }
 #pragma warning restore CS0162 // Unreachable code detected
       }
        public async Task DoSeed<TDbContext, TTenantInfo>(IServiceCollection services, IConfigurationRoot configurationRoot)
            where TDbContext : DbContext
            where TTenantInfo : class, ITenantEntity, new()
        {
            var data = new MultiTenantSeedData();
            configurationRoot.GetSection("MultiTenantData").Bind(data);
            services.AddSingleton(data);

            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var seedData = scope.ServiceProvider.GetRequiredService<MultiTenantSeedData>();
                _logger.LogDebug("Retrieved seed data for {TenantCount} tenant(s).", seedData.Tenants.Count, seedData);

                using (var context = scope.ServiceProvider.GetRequiredService<TDbContext>())
                {
                    foreach (var tenant in seedData.Tenants)
                    {
                        var entity = new TTenantInfo()
                        {
                            Id = tenant.Id,
                            Identifier = tenant.Identifier,
                            ConnectionString = tenant.ConnectionString,
                            Name = tenant.Name,
                            Items = tenant.Items
                        };

                        var dbTenant = await context.Set<TTenantInfo>().FirstOrDefaultAsync(t => t.Id == tenant.Id);

                        if (dbTenant != null)
                        {
                            dbTenant.Identifier = entity.Identifier;
                            dbTenant.ConnectionString = entity.ConnectionString;
                            dbTenant.Name = entity.Name;
                            dbTenant.Items = tenant.Items;

                            var updated = await context.SaveChangesAsync();

                            if (updated > 0)
                            {
                                _logger.LogDebug("Tenant {TenantId} already exists and was updated to match the seed data.", tenant.Id, tenant);
                            }
                            else
                            {
                                _logger.LogDebug("Tenant {TenantId} already exists.", tenant.Id, tenant);
                            }
                        }
                        else
                        {
                            context.Set<TTenantInfo>().Add(entity);
                            await context.SaveChangesAsync();

                            _logger.LogDebug("Tenant {TenantId} was added.", entity.Id, entity);
                        }
                    }
                }
            }

            _logger.LogInformation("Seeded {SourceContext}.");
        }
    }
}
