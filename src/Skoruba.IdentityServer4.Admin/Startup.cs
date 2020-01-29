using System.IdentityModel.Tokens.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Helpers;
using Skoruba.IdentityServer4.Admin.Configuration;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using System;
using Microsoft.EntityFrameworkCore;
using Skoruba.MultiTenant;
using Skoruba.MultiTenant.IdentityServer;
using Skoruba.MultiTenant.Finbuckle.Strategies;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Finbuckle;

namespace Skoruba.IdentityServer4.Admin
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            HostingEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddSingleton(rootConfiguration);

            // Register tenant configuration after root configuration
            RegisterMultiTenantConfiguration(services);

            // Add DbContexts for Asp.Net Core Identity, Logging and IdentityServer - Configuration store and Operational store
            RegisterDbContexts(services);

            // Add Asp.Net Core Identity Configuration and OpenIdConnect auth as well
            RegisterAuthentication(services);

            // Add HSTS options
            RegisterHstsOptions(services);

            // Add exception filters in MVC
            services.AddMvcExceptionFilters();

            // Add all dependencies for IdentityServer Admin
            services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

            // Add all dependencies for Asp.Net Core Identity
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddAdminAspNetIdentityServices<AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<string>, string, RoleDto<string>, string, string, string,
                                UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                                UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                                RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>();

            // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
            // Including settings for MVC and Localization
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddMvcWithLocalization<UserDto<string>, string, RoleDto<string>, string, string, string,
                UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                RoleClaimsDto<string>>(Configuration);

            // Add authorization policies for MVC
            RegisterAuthorization(services);

            // Add audit logging
            services.AddAuditEventLogging<AdminAuditLogDbContext, AuditLog>(Configuration);

            services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminIdentityDbContext, AdminLogDbContext, AdminAuditLogDbContext>(Configuration, rootConfiguration.AdminConfiguration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Add custom security headers
            app.UseSecurityHeaders();

            app.UseStaticFiles();

            UsePreAuthenticationMultitenantMiddleware(app);

            UseAuthentication(app);

            UsePostAuthenticationMultitenantMiddleware(app);

            // Use Localization
            app.ConfigureLocalization();

            app.UseRouting();
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
            services.RegisterDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext>(Configuration);
        }

        public virtual void RegisterAuthentication(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddAuthenticationServices<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(rootConfiguration.AdminConfiguration);
        }

        public virtual void RegisterAuthorization(IServiceCollection services)
        {
            var rootConfiguration = CreateRootConfiguration();
            services.AddAuthorizationPolicies(rootConfiguration);
        }
        public virtual void RegisterMultiTenantConfiguration(IServiceCollection services)
        {
            var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();

            if (configuration.MultiTenantEnabled)
            {
                services.AddMultiTenantConfiguration<SkorubaTenantContext>(configuration)
                    // 
                    // Add your multi tenant implementation services here
                    //
                    // register the default finbuckle multitenant services
                    .WithFinbuckleMultiTenant()
                    // custom store
                    .WithEFCacheStore(options => options.UseSqlServer(Configuration.GetConnectionString("TenantsDbConnection")))
                    // custom strategy to get tenant from form data at login
                    .WithStrategy<ClaimsStrategy>(ServiceLifetime.Singleton);
            }
            else
            {
                services.AddSingleTenantConfiguration();
            }
        }
        public virtual void UseAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
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

        public virtual void UsePreAuthenticationMultitenantMiddleware(IApplicationBuilder app)
        {
            var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();
            if (configuration.MultiTenantEnabled)
            {
                // if your multienant implementation requires any middleware 
                // to be configured BEFORE the authentication middleware
                // then put it here.
                //
                // for the default Finbuckle implementation we are not
                // registering anything here becuase we are relying on
                // the ClaimsStrategy which requires the middleware
                // to come after the authentication middleware.
            }
        }
        public virtual void UsePostAuthenticationMultitenantMiddleware(IApplicationBuilder app)
        {
            var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();
            if (configuration.MultiTenantEnabled)
            {
                // if your multienant implementation requires any middleware 
                // to be configured AFTER the authentication middleware
                // then put it here.
                //
                // for the default Finbuckle implementation we are registering
                // their middleware here.  The ClaimsStrategy requires the
                // middleware to come after the authentication middleware.
                app.UseMultiTenant();
            }
        }
        protected IRootConfiguration CreateRootConfiguration()
        {
            var rootConfiguration = new RootConfiguration();
            Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);
            Configuration.GetSection(ConfigurationConsts.IdentityDataConfigurationKey).Bind(rootConfiguration.IdentityDataConfiguration);
            Configuration.GetSection(ConfigurationConsts.IdentityServerDataConfigurationKey).Bind(rootConfiguration.IdentityServerDataConfiguration);
            return rootConfiguration;
        }
    }
}