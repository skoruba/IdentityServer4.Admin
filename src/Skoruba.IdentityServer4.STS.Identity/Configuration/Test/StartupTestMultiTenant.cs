using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Finbuckle;
using Skoruba.MultiTenant.Finbuckle.Strategies;
using Skoruba.MultiTenant.IdentityServer;
using Skoruba.MultiTenant.Stores;
using System;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration.Test
{
    public class StartupTestMultiTenant : Startup
    {
        public StartupTestMultiTenant(IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
        {

        }

        public override void RegisterDbContexts(IServiceCollection services)
        {
            services.RegisterDbContextsStaging<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext>();
        }

        public override void ConfigureMultiTenantServices(IServiceCollection services)
        {
            var tenantDatabaseName = Guid.NewGuid().ToString();
            var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();
 
            services.AddMultiTenantConfiguration<SkorubaTenantContext>(configuration)
                // dont require tenant resolution for identity endpoints
                .RegisterTenantIsRequiredValidation<TenantNotRequiredForIdentityServerEndpoints>()
                // 
                // Add multi tenant implementation services here
                //
                // register the default finbuckle multitenant services
                // include configuration settings for the FormStrategy
                .WithFinbuckleMultiTenant(Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration))
                // custom store
                .WithEFCacheStore(options => options.UseInMemoryDatabase(tenantDatabaseName))
                // custom strategy to get tenant from user claims
                .WithStrategy<ClaimsStrategy>(ServiceLifetime.Singleton)
                // if that fails, then use custom strategy to get tenant from form data at login
                .WithStrategy<FormStrategy>(ServiceLifetime.Singleton);

            // seed tenant for integration tests
            var tenantStore = services.BuildServiceProvider().GetService<EFCoreStoreDbContext>();
            tenantStore.TenantInfo.Add(new TenantEntity() { Id = Guid.NewGuid().ToString(), Identifier = "0000", Name = "Test", ConnectionString = "na" });
            tenantStore.SaveChanges();
        }

        public override void ConfigureMultiTenantMiddleware(IApplicationBuilder app)
        {
            // configure default multitenant middleware before authentication
            app.UseMultiTenant();
        }
    }
}