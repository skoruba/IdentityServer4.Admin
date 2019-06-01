using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

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

                await EnsureSeedIdentityServerData(context, rootConfiguration.ClientDataConfiguration);
                await EnsureSeedIdentityData(userManager, roleManager, rootConfiguration.UserDataConfiguration);
            }
        }

        /// <summary>
        /// Generate default admin user / role
        /// </summary>
        private static async Task EnsureSeedIdentityData<TUser, TRole>(UserManager<TUser> userManager,
            RoleManager<TRole> roleManager, IUserDataConfiguration userDataConfiguration)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            if (! await roleManager.Roles.AnyAsync()) { 
                // adding roles from seed
                foreach (var r in userDataConfiguration.Roles)
                {
                    if (! await roleManager.RoleExistsAsync(r.Name))
                    {
                        var role = new TRole
                        {
                            Name = r.Name
                        };

                        var res = await roleManager.CreateAsync(role);

                        if (res.Succeeded)
                        {
                            foreach (var c in  r.Claims)
                            {
                                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(c.Type, c.Value));
                            }
                        }
                    }
                }
            }

            if (!await userManager.Users.AnyAsync())
            {
                // adding users from seed
                foreach (var u in userDataConfiguration.Users)
                {
                    var us = new TUser
                    {
                        UserName = u.Username,
                        Email = u.Email,
                        EmailConfirmed = true
                    };


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
        private static async Task EnsureSeedIdentityServerData<TIdentityServerDbContext>(TIdentityServerDbContext context, IClientDataConfiguration clientDataConfiguration)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        {
            if (!context.IdentityResources.Any())
            {
                foreach (var resource in clientDataConfiguration.IdentityResources)
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in clientDataConfiguration.ApiResources)
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
                foreach (var client in clientDataConfiguration.Clients)
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
