using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Configuration.Identity;
using Skoruba.IdentityServer4.Admin.Configuration.IdentityServer;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores;

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
                var tenantManager = scope.ServiceProvider.GetService<ITenantManager>();

                await EnsureSeedIdentityServerData(context, rootConfiguration.AdminConfiguration);
                await EnsureSeedIdentityData(userManager, roleManager, tenantManager);
            }
        }

        /// <summary>
        /// Generate default admin user / role
        /// </summary>
        private static async Task EnsureSeedIdentityData<TUser, TRole>(UserManager<TUser> userManager,
            RoleManager<TRole> roleManager, ITenantManager tenantManager)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            string tenantCode = "0000";
            string tenantId = "2EBF8B61-BF2F-47C3-8D19-25C59DE646AD";
            TUser user;

            // Create Tenant for Multi Tenant usage
            if (userManager.IsMultiTenant())
            {
                if (!await tenantManager.TenantCodeExistsAsync(tenantCode))
                {
                    var tenant = new Tenant
                    {
                        Name = "Identity Administration",
                        Code = tenantCode,
                        Id = tenantId,
                        IsActive = true,
                        DatabaseName = "N/A",
                        DomainName = "localhost"
                    };
                    await tenantManager.CreateAsync(tenant);
                }
            }

            // Create admin role
            if (!await roleManager.RoleExistsAsync(AuthorizationConsts.AdministrationRole))
            {
                var role = new TRole { Name = AuthorizationConsts.AdministrationRole };

                await roleManager.CreateAsync(role);
            }

            // Create admin user
            if (userManager.IsMultiTenant())
            {
                if (await userManager.FindByNameAsync(tenantCode, Users.AdminUserName) != null) return;
            }
            else
            {
                if (await userManager.FindByNameAsync(Users.AdminUserName) != null) return;
            }
            user = new TUser
            {
                UserName = Users.AdminUserName,
                Email = Users.AdminEmail,
                EmailConfirmed = true
            };

            if (userManager.IsMultiTenant())
            {
                (user as MultiTenantUserIdentity).TenantId = tenantId;
            }

            var result = await userManager.CreateAsync(user, Users.AdminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, AuthorizationConsts.AdministrationRole);
            }
        }

        /// <summary>
        /// Generate default clients, identity and api resources
        /// </summary>
        private static async Task EnsureSeedIdentityServerData<TIdentityServerDbContext>(TIdentityServerDbContext context, IAdminConfiguration adminConfiguration)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Clients.GetAdminClient(adminConfiguration).ToList())
                {
                    await context.Clients.AddAsync(client.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                var identityResources = ClientResources.GetIdentityResources().ToList();

                foreach (var resource in identityResources)
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in ClientResources.GetApiResources(adminConfiguration).ToList())
                {
                    await context.ApiResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }
        }
    }
}