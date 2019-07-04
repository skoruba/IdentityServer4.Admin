using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.STS.Identity.DependencyInjection;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators;

namespace Skoruba.IdentityServer4.STS.Identity.DependencyInjection
{
    public static partial class BuilderExtensions
    {
        public static IBuilder AddSingleTenantConfiguration
            (this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILogger logger,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<IdentityOptions> identityOptions,
            Action<IdentityServerOptions> identityServerOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions)
        {
            var builder = services
                .AddCustomConfiguration
                  <AdminIdentityDbContext, UserIdentity, UserIdentityRole>
                    (hostingEnvironment, identityDbContextOptions)
                .AddSingleTenantUserIdentity
                    <AdminIdentityDbContext, UserIdentity, UserIdentityRole>
                    (identityOptions)
                .AddIdentityServer
                    <UserIdentity, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext>
                    (configuration, hostingEnvironment, logger, identityServerOptions, configurationStoreOptions, operationalStoreOptions)
                .AddMvcWithLocalization
                    <UserIdentity, string>();

            ////Uncomment to require 2fa for users
            //builder.AddUserValidator<UserIdentity, MightRequireTwoFactorAuthentication<UserIdentity>>();

            return builder;
        }

        private static IBuilder AddSingleTenantUserIdentity
            <TIdentityDbContext, TUserIdentity, TUserIdentityRole>
            (this IBuilder builder, Action<IdentityOptions> options)
            where TIdentityDbContext : DbContext
            where TUserIdentity : class
            where TUserIdentityRole : class
        {
            builder.Services
                .AddIdentity<TUserIdentity, TUserIdentityRole>(options)
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddDefaultTokenProviders();

            return builder;
        }
    }
}