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
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;
using IdentityServer4.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.STS.Identity.Services;
using System.Linq;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators;

namespace Skoruba.IdentityServer4.STS.Identity.DependencyInjection
{
    public static partial class BuilderExtensions
    {
        public static IBuilder AddMultiTenantConfiguration
            (this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILogger logger,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<IdentityOptions> identityOptions,
            Action<IdentityServerOptions> identityServerOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions)
        {
            services.AddMemoryCache();

            var builder = services
                .AddCustomConfiguration
                    <MultiTenantUserIdentityDbContext, MultiTenantUserIdentity, UserIdentityRole>
                    (hostingEnvironment, identityDbContextOptions)
                .AddMultiTenantUserIdentity
                    <MultiTenantUserIdentityDbContext, MultiTenantUserIdentity, UserIdentityRole>
                    (identityOptions)
                .AddMultiTenantIdentityServer
                    <MultiTenantUserIdentity, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext>
                    (configuration, hostingEnvironment, logger, identityServerOptions, configurationStoreOptions, operationalStoreOptions)
                .AddMvcWithLocalization
                    <MultiTenantUserIdentity, string>();

            builder.Services.Remove(builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IUserValidator<MultiTenantUserIdentity>)));
            builder.AddUserValidator<MultiTenantUserIdentity, MultiTenantUserValidator>();
            builder.AddUserValidator<MultiTenantUserIdentity, RequireTenant>();
            builder.AddUserValidator<MultiTenantUserIdentity, MightRequireTwoFactorAuthentication<MultiTenantUserIdentity>>();
            return builder;
        }

        private static IBuilder AddMultiTenantUserIdentity
            <TIdentityDbContext, TUserIdentity, TUserIdentityRole>
            (this IBuilder builder, Action<IdentityOptions> options)
            where TIdentityDbContext : DbContext
            where TUserIdentity : class
            where TUserIdentityRole : class
        {
            builder.Services
                .AddIdentity<TUserIdentity, TUserIdentityRole>(options)
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddUserStore<MultiTenantUserStore>()
                .AddSignInManager<MultiTenantSigninManager<TUserIdentity>>()
                .AddUserManager<MultiTenantUserManager<TUserIdentity>>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<ITenantStore, TenantStore>();
            builder.Services.AddScoped<ITenantManager, TenantManager>();

            return builder;
        }

        /// <summary>
        /// Adds IdentityServer4 and registers the dbContexts for configuration and operation
        /// </summary>
        /// <typeparam name="TUserIdentity"></typeparam>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="identityServerOptions"></param>
        /// <param name="configurationStoreOptions"></param>
        /// <param name="operationalStoreOptions"></param>
        /// <returns></returns>
        public static IBuilder AddMultiTenantIdentityServer
            <TUserIdentity, TConfigurationDbContext, TPersistedGrantDbContext>
            (this IBuilder builder,
                IConfiguration configuration,
                IHostingEnvironment hostingEnvironment, ILogger logger,
                Action<IdentityServerOptions> identityServerOptions,
                Action<ConfigurationStoreOptions> configurationStoreOptions,
                Action<OperationalStoreOptions> operationalStoreOptions)
            where TUserIdentity : class
            where TConfigurationDbContext : DbContext, IConfigurationDbContext
            where TPersistedGrantDbContext : DbContext, IPersistedGrantDbContext
        {
            builder.Services.AddIdentityServer(identityServerOptions)
                .AddAspNetIdentity<TUserIdentity>()
                .AddIdentityServerStoresWithDbContexts<TConfigurationDbContext, TPersistedGrantDbContext>(configuration, hostingEnvironment, logger, configurationStoreOptions, operationalStoreOptions)
                .AddProfileService<MultiTenantProfileService>();

            return builder;
        }
    }
}