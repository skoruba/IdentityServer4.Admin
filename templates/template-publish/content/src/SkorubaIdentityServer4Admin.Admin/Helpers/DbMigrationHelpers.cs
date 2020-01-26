using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using SkorubaIdentityServer4Admin.Admin.Configuration;
using SkorubaIdentityServer4Admin.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace SkorubaIdentityServer4Admin.Admin.Helpers
{
    public static class DbMigrationHelpers
    {
        /// <summary>
        /// Generate migrations before running this method, you can use these steps bellow:
        /// https://github.com/skoruba/IdentityServer4.Admin#ef-core--data-access
        /// </summary>
        /// <param name="host"></param>      
        public static async Task EnsureSeedData<TIdentityServerDbContext, TIdentityDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TUser, TRole>(IHost host)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLogDbContext: DbContext, IAuditLoggingDbContext<AuditLog>
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                await EnsureDatabasesMigrated<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext>(services);
                await EnsureSeedData<TIdentityServerDbContext, TUser, TRole>(services);
            }
        }

        public static async Task EnsureDatabasesMigrated<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext>(IServiceProvider services)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext
            where TConfigurationDbContext : DbContext
            where TLogDbContext : DbContext
            where TAuditLogDbContext: DbContext
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

                using (var context = scope.ServiceProvider.GetRequiredService<TAuditLogDbContext>())
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

                await EnsureSeedIdentityServerData(context, rootConfiguration.IdentityServerDataConfiguration);
                await EnsureSeedIdentityData(userManager, roleManager, rootConfiguration.IdentityDataConfiguration);
            }
        }

        /// <summary>
        /// Generate default admin user / role
        /// </summary>
        private static async Task EnsureSeedIdentityData<TUser, TRole>(UserManager<TUser> userManager,
            RoleManager<TRole> roleManager, IdentityDataConfiguration identityDataConfiguration)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                // adding roles from seed
                foreach (var r in identityDataConfiguration.Roles)
                {
                    if (!await roleManager.RoleExistsAsync(r.Name))
                    {
                        var role = new TRole
                        {
                            Name = r.Name
                        };

                        var result = await roleManager.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            foreach (var claim in r.Claims)
                            {
                                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, claim.Value));
                            }
                        }
                    }
                }
            }

            if (!await userManager.Users.AnyAsync())
            {
                // adding users from seed
                foreach (var user in identityDataConfiguration.Users)
                {
                    var identityUser = new TUser
                    {
                        UserName = user.Username,
                        Email = user.Email,
                        EmailConfirmed = true
                    };

                    // if there is no password we create user without password
                    // user can reset password later, because accounts have EmailConfirmed set to true
                    var result = !string.IsNullOrEmpty(user.Password)
                        ? await userManager.CreateAsync(identityUser, user.Password)
                        : await userManager.CreateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        foreach (var claim in user.Claims)
                        {
                            await userManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim(claim.Type, claim.Value));
                        }

                        foreach (var role in user.Roles)
                        {
                            await userManager.AddToRoleAsync(identityUser, role);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate default clients, identity and api resources
        /// </summary>
        private static async Task EnsureSeedIdentityServerData<TIdentityServerDbContext>(TIdentityServerDbContext context, IdentityServerDataConfiguration identityServerDataConfiguration)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        {
            if (!context.IdentityResources.Any())
            {
                foreach (var resource in identityServerDataConfiguration.IdentityResources)
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in identityServerDataConfiguration.ApiResources)
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
                foreach (var client in identityServerDataConfiguration.Clients)
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






