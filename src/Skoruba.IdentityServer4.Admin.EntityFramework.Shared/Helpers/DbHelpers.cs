using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Helpers
{
    public class DbHelpers
    {
        public static void ResetDb(IServiceProvider provider)
        {
            using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ResetDb(scope.ServiceProvider.GetService<AdminIdentityDbContext>());
                ResetDb(scope.ServiceProvider.GetService<IdentityServerPersistedGrantDbContext>());
                ResetDb(scope.ServiceProvider.GetService<IdentityServerConfigurationDbContext>());
                ResetDb(scope.ServiceProvider.GetService<IdentityServerDataProtectionDbContext>());
                ResetDb(scope.ServiceProvider.GetService<AdminLogDbContext>());
                ResetDb(scope.ServiceProvider.GetService<AdminAuditLogDbContext>());
            }
        }

        private static void ResetDb(DbContext context)
        {
            if (context == null)
            {
                return;
            }

            using (context)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}