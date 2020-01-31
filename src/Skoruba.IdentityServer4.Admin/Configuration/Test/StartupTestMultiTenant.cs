using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Helpers;
using Skoruba.IdentityServer4.Admin.Middlewares;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Finbuckle;
using Skoruba.MultiTenant.Finbuckle.Strategies;
using Skoruba.MultiTenant.IdentityServer;
using Skoruba.MultiTenant.Stores;
using System;

namespace Skoruba.IdentityServer4.Admin.Configuration.Test
{
    public class StartupTestMultiTenant : Startup
    {
        public StartupTestMultiTenant(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        public override void RegisterDbContexts(IServiceCollection services)
        {
            services.RegisterDbContextsStaging<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext>();
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

        public override void ConfigureMultiTenantServices(IServiceCollection services)
        {
            var tenantDatabaseName = Guid.NewGuid().ToString();
            var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();

            services.AddMultiTenantConfiguration<SkorubaTenantContext>(configuration)
                // 
                // Add your multi tenant implementation services here
                //
                // register the default finbuckle multitenant services
                .WithFinbuckleMultiTenant()
                // custom store
                .WithEFCacheStore(options => options.UseInMemoryDatabase(tenantDatabaseName))
                // custom strategy to get tenant from form data at login
                .WithStrategy<ClaimsStrategy>(ServiceLifetime.Singleton);

            // seed tenant for integration tests
            var tenantStore = services.BuildServiceProvider().GetService<EFCoreStoreDbContext>();
            tenantStore.TenantInfo.Add(new TenantEntity() { Id = Guid.NewGuid().ToString(), Identifier = "0000", Name = "Test", ConnectionString = "na" });
            tenantStore.SaveChanges();
        }
        public override void ConfigureMultiTenantMiddleware(IApplicationBuilder app)
        {
            // configure custom multitenant middleware for claims after authentication
            app.UseMultiTenant();
        }
    }
}
