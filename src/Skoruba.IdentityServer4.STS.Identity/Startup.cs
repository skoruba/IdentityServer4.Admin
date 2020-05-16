﻿using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Interfaces;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Skoruba.MultiTenant;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Finbuckle;
using Skoruba.MultiTenant.Finbuckle.Strategies;
using Skoruba.MultiTenant.IdentityServer;

namespace Skoruba.IdentityServer4.STS.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddSingleton(rootConfiguration);

            // Register tenant configuration after root configuration
            ConfigureMultiTenantServices(services);
            // Register DbContexts for IdentityServer and Identity
            RegisterDbContexts(services);

            // Save data protection keys to db, using a common application name shared between Admin and STS
            services.AddDataProtection()
                .SetApplicationName("Skoruba.IdentityServer4")
                .PersistKeysToDbContext<IdentityServerDataProtectionDbContext>();

            // Add email senders which is currently setup for SendGrid and SMTP
            services.AddEmailSenders(Configuration);

            // Add services for authentication, including Identity model and external providers
            RegisterAuthentication<UserIdentity, string>(services);

            // Add HSTS options
            RegisterHstsOptions(services);

            // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
            // Including settings for MVC and Localization
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddMvcWithLocalization<UserIdentity, string>(Configuration);

            // Add authorization policies for MVC
            RegisterAuthorization(services);

            services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminIdentityDbContext, IdentityServerDataProtectionDbContext>(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCookiePolicy();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Add custom security headers
            app.UseSecurityHeaders();

            app.UseStaticFiles();

            app.UseRouting();

            UseAuthentication(app);

            // If using a claims strategy then this must come after authentication;
            // otherwise, this should go before authentication.
            ConfigureMultiTenantMiddleware(app);

            app.UseMvcLocalizationServices();

            app.UseAuthorization();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapDefaultControllerRoute();
                endpoint.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }

        public virtual void RegisterDbContexts(IServiceCollection services)
        {
            services.RegisterDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, IdentityServerDataProtectionDbContext>(Configuration);
        }

        public virtual void RegisterAuthentication<TUser, TKey>(IServiceCollection services)
            where TUser : IdentityUser<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            services.AddActiveDirectoryAuth<TUser, TKey>(Configuration);
            services.AddAuthenticationServices<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(Configuration);
            services.AddIdentityServer<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, UserIdentity>(Configuration);
        }

        public virtual void RegisterAuthorization(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddAuthorizationPolicies(rootConfiguration);
        }
        public virtual void ConfigureMultiTenantServices(IServiceCollection services)
        {
            var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();

            if (configuration.MultiTenantEnabled)
            {
                services.AddMultiTenantConfiguration<SkorubaTenantContext>(configuration)
                    // do not require tenant resolution for identity endpoints
                    .RegisterTenantIsRequiredValidation<TenantNotRequiredForIdentityServerEndpoints>()
                    // 
                    // Add your multi tenant implementation services here
                    // Changes here should also be considered in 
                    // Skoruba.IdentityServer4.STS.Identity.Configuration.Test.StartupTestMultiTenant
                    //
                    // register the default finbuckle multitenant services
                    // include configuration settings for the FormStrategy
                    .WithFinbuckleMultiTenant(Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration))
                    // custom store
                    .WithEFCacheStore(options => options.UseSqlServer(Configuration.GetConnectionString("TenantsDbConnection")))
                    // custom strategy to get tenant from user claims
                    .WithStrategy<ClaimsStrategy>(ServiceLifetime.Singleton)
                    // if that fails, then use custom strategy to get tenant from form data at login
                    .WithStrategy<FormStrategy>(ServiceLifetime.Singleton);
            }
            else
            {
                services.AddSingleTenantConfiguration();
            }
        }

        public virtual void UseAuthentication(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }

        public virtual void RegisterHstsOptions(IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }


        public virtual void ConfigureMultiTenantMiddleware(IApplicationBuilder app)
        {
            var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();
            if (configuration.MultiTenantEnabled)
            {
                // for the default Finbuckle implementation we are registering
                // their middleware here.  This middleware will use the
                // resolution strategy defined in the services.
                app.UseMultiTenant();
            }
        }
        protected IRootConfiguration CreateRootConfiguration()
        {
            var rootConfiguration = new RootConfiguration();
            Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);
            Configuration.GetSection(ConfigurationConsts.RegisterConfigurationKey).Bind(rootConfiguration.RegisterConfiguration);
            Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Bind(rootConfiguration.MultiTenantConfiguration);
            return rootConfiguration;
        }
    }
}
