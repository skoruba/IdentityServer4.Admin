using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Finbuckle.Strategies;
using Skoruba.MultiTenant.IdentityServer;
using System;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration.Test
{
    public class StartupTestSingleTenant : Startup
    {
        public StartupTestSingleTenant(IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
        {
        }

        public override void RegisterDbContexts(IServiceCollection services)
        {
            services.RegisterDbContextsStaging<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext>();
        }
        public override void RegisterMultiTenantConfiguration(IServiceCollection services)
        {
            var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();
            services.AddMultiTenantConfiguration<SkorubaSingleTenantContext>(configuration);
        }

        public override void UsePreAuthenticationMultitenantMiddleware(IApplicationBuilder app)
        {
            
        }

        public override void UsePostAuthenticationMultitenantMiddleware(IApplicationBuilder app)
        {
            
        }
    }
}