using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

        public override void RegisterDbContexts(IServiceCollection services)
        {
            services.RegisterDbContexts<
                AdminIdentityDbContext,
                IdentityServerConfigurationDbContext,
                IdentityServerPersistedGrantDbContext,
                AdminLogDbContext,
                AdminAuditLogDbContext,
                IdentityServerDataProtectionDbContext>(Configuration);

            EnsureDatabaseCreated(services);
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

        private void EnsureDatabaseCreated(IServiceCollection services)
        {
            ServiceProvider provider = services.BuildServiceProvider();

            using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<IdentityServerPersistedGrantDbContext>())
                {
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<AdminIdentityDbContext>())
                {
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<IdentityServerConfigurationDbContext>())
                {
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<AdminLogDbContext>())
                {
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<AdminAuditLogDbContext>())
                {
                    context.Database.EnsureCreated();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<IdentityServerDataProtectionDbContext>())
                {
                    context.Database.EnsureCreated();
                }
            }
        }
    }
}
