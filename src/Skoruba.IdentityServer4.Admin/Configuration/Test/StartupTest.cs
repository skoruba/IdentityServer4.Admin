using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Helpers;
using Skoruba.IdentityServer4.Admin.Middlewares;

namespace Skoruba.IdentityServer4.Admin.Configuration.Test
{
    public class StartupTest : Startup
    {
        public StartupTest(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        public override void RegisterAuthentication(IServiceCollection services)
        {
            services.AddAuthenticationServicesStaging<AdminIdentityDbContext, UserIdentity, UserIdentityRole>();
        }

        public override void RegisterAuthorization(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddAuthorizationPolicies(rootConfiguration);
        }

        public override void UseAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
        }

        protected override void DoStartupPostProcessing(IApplicationBuilder app)
        {
            ResetDb(app.ApplicationServices);
        }

        private void ResetDb(IServiceProvider provider)
        {
            using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<IdentityServerPersistedGrantDbContext>())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<AdminIdentityDbContext>())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<IdentityServerConfigurationDbContext>())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<AdminLogDbContext>())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<AdminAuditLogDbContext>())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<IdentityServerDataProtectionDbContext>())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
            }
        }
    }
}
