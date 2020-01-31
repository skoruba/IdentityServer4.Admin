using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skoruba.DbMigrator.Abstractions;
using Skoruba.DbMigrator.Abstractions.Dependency;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.DbMigrator.MigrateAndSeed
{
    [DependsOn(typeof(MigrateAdminIdentityDbContext))]
    public class MigrateIdentityServerConfigurationDbContext : MigrateAndSeedBase
    {
        public MigrateIdentityServerConfigurationDbContext(ILogger<MigrateIdentityServerConfigurationDbContext> logger) : base(logger)
        {
        }

        public override Task Migrate(IServiceCollection services)
        {
            return MigrateDbContext<IdentityServerConfigurationDbContext>(services.BuildServiceProvider());
        }

        public override Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            return DoSeed<IdentityServerConfigurationDbContext>(services.BuildServiceProvider());
        }

        private async Task DoSeed<TDbContext>(ServiceProvider serviceProvider)
            where TDbContext : DbContext, IAdminConfigurationDbContext
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<TDbContext>())
                {
                    var rootConfiguration = scope.ServiceProvider.GetRequiredService<IRootConfiguration>();

                    var identityServerDataConfiguration = rootConfiguration.IdentityServerDataConfiguration;

                    _logger.LogDebug("Retrieved seed data.", identityServerDataConfiguration);

                    if (!context.IdentityResources.Any())
                    {
                        foreach (var resource in identityServerDataConfiguration.IdentityResources)
                        {
                            _logger.LogDebug("Adding Resource {Resource}.", resource.Name);
                            await context.IdentityResources.AddAsync(resource.ToEntity());
                        }

                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogDebug("Resources already exist.");
                    }
                    if (!context.ApiResources.Any())
                    {
                        foreach (var resource in identityServerDataConfiguration.ApiResources)
                        {
                            foreach (var s in resource.ApiSecrets)
                            {
                                s.Value = s.Value.ToSha256();
                            }

                            _logger.LogDebug("Adding ApiResource {ApiResource}.", resource.Name);
                            await context.ApiResources.AddAsync(resource.ToEntity());
                        }

                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogDebug("ApiResources already exist.");
                    }

                    if (!context.Clients.Any())
                    {
                        foreach (var client in identityServerDataConfiguration.Clients)
                        {
                            foreach (var secret in client.ClientSecrets)
                            {
                                secret.Value = secret.Value.ToSha256();
                            }

                            client.Claims = client.ClientClaims
                                .Select(c => new System.Security.Claims.Claim(c.Type, c.Value))
                                .ToList();

                            _logger.LogDebug("Adding Client {Client}.", client.ClientId);
                            await context.Clients.AddAsync(client.ToEntity());
                        }

                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogDebug("Clients already exist.");
                    }

                    _logger.LogDebug("Seeded {SourceContext}.");
                }
            }
        }
    }
}
