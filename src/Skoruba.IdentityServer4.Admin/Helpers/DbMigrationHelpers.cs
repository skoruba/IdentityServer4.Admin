using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.Configuration.SeedModels;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Helpers
{
    public static class DbMigrationHelpers
    {
        /// <summary>
        /// Generate migrations before running this method, you can use these steps bellow:
        /// https://github.com/skoruba/IdentityServer4.Admin#ef-core--data-access
        /// </summary>
        /// <param name="host"></param>
        public static async Task EnsureSeedData<TIdentityServerDbContext, TIdentityDbContext, TPersistedGrantDbContext, TLogDbContext, TUser, TRole>(IWebHost host)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                await EnsureDatabasesMigrated<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext>(services);
                await EnsureSeedData<TIdentityServerDbContext, TUser, TRole>(services);
            }
        }

        public static async Task EnsureDatabasesMigrated<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(IServiceProvider services)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext
            where TConfigurationDbContext : DbContext
            where TLogDbContext : DbContext
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<TPersistedGrantDbContext>())
                {
                    await context.Database.MigrateAsync();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<TIdentityDbContext>())
                {
                    await context.Database.MigrateAsync();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<TConfigurationDbContext>())
                {
                    await context.Database.MigrateAsync();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<TLogDbContext>())
                {
                    await context.Database.MigrateAsync();
                }
            }
        }

        public static async Task EnsureSeedData<TIdentityServerDbContext, TUser, TRole>(IServiceProvider serviceProvider)
        where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TIdentityServerDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TRole>>();
                var rootConfiguration = scope.ServiceProvider.GetRequiredService<IRootConfiguration>();
                var tenantManager = scope.ServiceProvider.GetRequiredService<ITenantManager>();
                var seedData = scope.ServiceProvider.GetRequiredService<IOptions<SeedData>>();

                await EnsureSeedIdentityServerData(context, seedData.Value, rootConfiguration.AdminConfiguration);
                await EnsureSeedIdentityData(userManager, roleManager, tenantManager, rootConfiguration, seedData.Value);
            }
        }

        /// <summary>
        /// Generate default admin user / role
        /// </summary>
        private static async Task EnsureSeedIdentityData<TUser, TRole>(UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            ITenantManager tenantManager,
            IRootConfiguration rootConfiguration,
            SeedData seedData)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                // adding roles from seed
                foreach (var r in seedData.Roles)
                {
                    var role = new TRole
                    {
                        Name = r.Name
                    };

                    var res = await roleManager.CreateAsync(role);

                    if (res.Succeeded)
                    {
                        foreach (var c in r.Claims)
                        {
                            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(c.Type, c.Value));
                        }
                    }
                }
            }

            if (!await tenantManager.Tenants.AnyAsync())
            {
                foreach (var tenant in seedData.Tenants)
                {
                    var t = new EntityFramework.Shared.Entities.Tenants.Tenant
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        IsActive = tenant.IsActive,
                        DatabaseName = tenant.DatabaseName,
                        Code = tenant.Code,
                        RequireTwoFactorAuthentication = tenant.RequireTwoFactorAuthentication
                    };
                    await tenantManager.CreateAsync(t);
                }
            }

            if (!await userManager.Users.AnyAsync())
            {
                // adding users from seed
                foreach (var u in seedData.Users)
                {
                    var us = new TUser
                    {
                        UserName = u.UserName,
                        Email = u.Email,
                        EmailConfirmed = u.EmailConfirmed,
                        TwoFactorEnabled = u.TwoFactorEnabled
                    };

                    if (userManager.IsMultiTenant())
                    {
                        (us as MultiTenantUserIdentity).TenantId = u.TenantId;
                        (us as MultiTenantUserIdentity).ApplicationId = u.ApplicationId;
                    }

                    // if there is no password we create user without password
                    // user can reset password later, because accounts have EmailConfirmed set to true
                    var res = u.Password != null ? await userManager.CreateAsync(us, u.Password) : await userManager.CreateAsync(us);

                    if (res.Succeeded)
                    {
                        foreach (var c in u.Claims)
                        {
                            await userManager.AddClaimAsync(us, new System.Security.Claims.Claim(c.Type, c.Value));
                        }

                        foreach (var r in u.Roles)
                        {
                            await userManager.AddToRoleAsync(us, r);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate default clients, identity and api resources
        /// </summary>
        private static async Task EnsureSeedIdentityServerData<TIdentityServerDbContext>(TIdentityServerDbContext context, ISeedData seedData, IAdminConfiguration adminConfiguration)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        {
            if (!context.IdentityResources.Any())
            {
                foreach (var resource in seedData.IdentityResources)
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in seedData.ApiResources)
                {
                    foreach (var s in resource.ApiSecrets)
                    {
                        s.Value = s.Value.ToSha256();
                    }

                    await context.ApiResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.Clients.Any())
            {
                foreach (var client in seedData.Clients)
                {
                    foreach (var secret in client.ClientSecrets)
                    {
                        secret.Value = secret.Value.ToSha256();
                    }

                    client.Claims = client.ClientClaims
                        .Select(c => new System.Security.Claims.Claim(c.Type, c.Value))
                        .ToList();

                    await context.Clients.AddAsync(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }
        }
    }
}