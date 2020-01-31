using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Skoruba.DbMigrator.Abstractions;
using Skoruba.DbMigrator.Abstractions.Dependency;
using Skoruba.IdentityServer4.Admin.Configuration;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.MultiTenant.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.DbMigrator.MigrateAndSeed
{
    [DependsOn(typeof(MigrateIdentityServerPersistedGrantDbContext))]
    public class MigrateAdminIdentityDbContext : MigrateAndSeedBase
    {
        public MigrateAdminIdentityDbContext(ILogger<MigrateAdminIdentityDbContext> logger) : base(logger)
        {
        }

        public override Task Migrate(IServiceCollection services)
        {
            return MigrateDbContext<AdminIdentityDbContext>(services.BuildServiceProvider());
        }

        public override Task Seed(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            return DoSeed<UserIdentity, UserIdentityRole>(services);
        }

        private async Task DoSeed<TUser, TRole>(IServiceCollection services)
            where TUser : UserIdentity, new()
            where TRole : UserIdentityRole, new()
        {
            IdentityDataConfiguration seeddata = null;

            using (var scope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var rootConfiguration = scope.ServiceProvider.GetRequiredService<IRootConfiguration>();
                seeddata = rootConfiguration.IdentityDataConfiguration;
                _logger.LogDebug("Retrieved seed data.", seeddata);
            }

            // The RoleStore and UserStore both require a TenantInfo
            // object to be injected.  This object is used to establish
            // the TenantId for new Users and new Roles.
            // This will require a loop of each tenant found in the 
            // seed data and to have that tenant added to the service
            // collection prior to seeding so that it can be injected.

            var tenantIds = seeddata.Roles.Select(a => a.TenantId)
                .Union(seeddata.Users.Select(a => a.TenantId));

            _logger.LogDebug("Seed data contains {TenantCount} tenant(s).", tenantIds.Count());

            foreach (var tenantId in tenantIds)
            {
                _logger.LogDebug("Seeding tenant {TenantId} data.", tenantId);

                // remove previously added TenantInfo from the service collection
                var tenantInfoToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TenantInfo));
                services.Remove(tenantInfoToRemove);

                // Add the new TenantInfo to the service collection.  We only need the tenant id.
                services.AddScoped(sp => new TenantInfo(tenantId, "na", "na", "na", new Dictionary<string, object>()));

                // Scope services and perform seeding 
                using (var scope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TRole>>();

                    foreach (var r in seeddata.Roles.Where(r => r.TenantId == tenantId))
                    {
                        // We are not currently using role ids
                        // to check if it exists, so we cannot
                        // reliably update role names.
                        var role = new TRole
                        {
                            Name = r.Name,
                            TenantId = r.TenantId
                        };
                        if (!await roleManager.RoleExistsAsync(role.Name))
                        {
                            _logger.LogDebug("Role {Role} does not exist, adding role.", role);
                            var result = await roleManager.CreateAsync(role);

                            if (result.Succeeded)
                            {
                                _logger.LogDebug("Role {Role} successfully added.", role);
                                foreach (var claim in r.Claims)
                                {
                                    await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, claim.Value));
                                    _logger.LogDebug("Claim {Claim} added for role {Role}.", claim, role);
                                }
                            }
                        }
                        else
                        {
                            // TODO: Update role claims to sync with seed data
                            _logger.LogDebug("Role {Role} exists.", role);

                        }
                    }

                    foreach (var u in seeddata.Users.Where(u => u.TenantId == tenantId))
                    {
                        var identityUser = new TUser
                        {
                            UserName = u.Username,
                            Email = u.Email,
                            EmailConfirmed = true,
                            TenantId = u.TenantId
                        };

                        // The tenant id is injected into the usermanager object
                        // so we know that this user is unique and belongs
                        // to the tenant we're looping on.
                        var dbUser = await userManager.FindByEmailAsync(u.Email);

                        if (dbUser == null)
                        {
                            // if there is no password we create user without password
                            // user can reset password later, because accounts have EmailConfirmed set to true
                            _logger.LogDebug("User {User} does not exist, adding user.", u.Email);
                            var result = !string.IsNullOrEmpty(u.Password)
                                ? await userManager.CreateAsync(identityUser, u.Password)
                                : await userManager.CreateAsync(identityUser);

                            if (result.Succeeded)
                            {
                                _logger.LogDebug("User {User} sucessfully added.", u.Email);
                                foreach (var claim in u.Claims)
                                {
                                    await userManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim(claim.Type, claim.Value));
                                    _logger.LogDebug("Added claim {Claim} for user {User}",  claim.Type, u.Email);
                                }

                                foreach (var role in u.Roles)
                                {
                                    await userManager.AddToRoleAsync(identityUser, role);
                                    _logger.LogDebug("Added role {Role} for user {User}", role, u.Email);
                                }
                            }
                            else
                            {
                                _logger.LogError("User {User} not added. {Error}", u.Email, result.Errors.First().Description);
                            }
                        }
                        else
                        {
                            // Since the user already exists in the db
                            // we are going to update the username in case
                            // the seed data was changed.
                            _logger.LogDebug("User {User} already exists.", u.Email);
                            dbUser.UserName = identityUser.UserName;

                            var result = await userManager.UpdateAsync(dbUser);

                            // TODO: update user claims to sync with seed data
                        }
                    }
                }
            }

            _logger.LogInformation("Seeded {SourceContext}.");
        }
    }
}
