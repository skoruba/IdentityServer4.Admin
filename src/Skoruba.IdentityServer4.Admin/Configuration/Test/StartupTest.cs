﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.Admin.Configuration.Test
{
	public class StartupTest : Startup
    {
        public StartupTest(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        public override void ConfigureUIOptions(IdentityServer4AdminUIOptions options)
        {
            // Applies configuration from appsettings.
            options.ApplyConfiguration(Configuration);
            options.ApplyConfiguration(HostingEnvironment);

            // Use staging DbContexts and auth services.
            options.IsStaging = true;
        }

        //public override void RegisterDbContexts(IServiceCollection services)
        //{
        //    services.RegisterDbContextsStaging<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext, IdentityServerDataProtectionDbContext>();
        //}

        //public override void RegisterAuthentication(IServiceCollection services)
        //{
        //    services.AddAuthenticationServicesStaging<AdminIdentityDbContext, UserIdentity, UserIdentityRole>();
        //}

        //public override void RegisterAuthorization(IServiceCollection services)
        //{
        //    var rootConfiguration = CreateRootConfiguration();
        //    services.AddAuthorizationPolicies(rootConfiguration);
        //}

        //public override void UseAuthentication(IApplicationBuilder app)
        //{
        //    app.UseAuthentication();
        //    app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
        //}
    }
}
