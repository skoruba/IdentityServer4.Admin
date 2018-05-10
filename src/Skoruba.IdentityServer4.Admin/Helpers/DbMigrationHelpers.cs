using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.Configuration;
using Skoruba.IdentityServer4.Admin.Data.DbContexts;

namespace Skoruba.IdentityServer4.Admin.Helpers
{
    public static class DbMigrationHelpers
    {
        public static void MigrateDbContexts(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<AdminDbContext>().Database.Migrate();
                EnsureSeedData(serviceScope.ServiceProvider.GetService<AdminDbContext>());
            }
        }

        private static void EnsureSeedData(AdminDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Clients.GetAdminClient().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                var identityResources = ClientResources.GetIdentityResources().ToList();

                foreach (var resource in identityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in ClientResources.GetApiResources().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}
