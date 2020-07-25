﻿using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.Api.Helpers;
using Skoruba.IdentityServer4.Admin.Api.Middlewares;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entitites.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Stores;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Shared.Configuration.Identity;

namespace Skoruba.IdentityServer4.Admin.Api.Configuration.Test
{
    public class StartupTest : Startup
    {
        public StartupTest(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        public override void RegisterDbContexts(IServiceCollection services)
        {
            services.RegisterDbContextsStaging<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext, MultiTenantDbContext, IdentityServerDataProtectionDbContext>();
        }

        public override void RegisterAuthentication(IServiceCollection services)
        {
            var loginConfiguration = Configuration.GetSection(nameof(LoginConfiguration)).Get<LoginConfiguration>();
            var registerConfiguration = Configuration.GetSection(nameof(RegisterConfiguration)).Get<RegisterConfiguration>();

            services.AddIdentity<UserIdentity, UserIdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = loginConfiguration.RequireUniqueEmail;
                    options.SignIn.RequireConfirmedAccount = registerConfiguration.RequireConfirmedAccount;
                })
                .AddEntityFrameworkStores<AdminIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(IdentityServerAuthenticationDefaults.AuthenticationScheme);
        }

        public override void RegisterAuthorization(IServiceCollection services)
        {
            services.AddAuthorizationPolicies();
        }

        public override void UseAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
        }
    }
}