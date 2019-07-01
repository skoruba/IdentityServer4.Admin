using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.IO;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Storage;

namespace Skoruba.IdentityServer4.DependencyInjection
{
    public static partial class SkorubaIdentityServerAdminBuilderExtensions
    {
        #region Single Tenant Configuration

        // Generic implementation
        public static ISkorubaIdentityServerAdminBuilder AddCustomConfiguration
            <TAdminIdentityDbContext, TIdentityServerConfigurationDbContext, TIdentityServerPersistedGrantDbContext, TAdminLogDbContext>
            (this IServiceCollection services,
            IHostingEnvironment hostingEnvironment,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions,
            Action<DbContextOptionsBuilder> logDbContextOptions)
            where TAdminIdentityDbContext : DbContext
            where TIdentityServerConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TIdentityServerPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TAdminLogDbContext : DbContext, IAdminLogDbContext
        {
            var builder = new SkorubaIdentityServerAdminBuilder(services);

            builder.Services.AddDbContexts
                <TAdminIdentityDbContext, TIdentityServerConfigurationDbContext, TIdentityServerPersistedGrantDbContext, TAdminLogDbContext>
                (hostingEnvironment, identityDbContextOptions, configurationStoreOptions, operationalStoreOptions, logDbContextOptions);

            return builder;
        }

        #endregion Single Tenant Configuration

        #region Add DbContexts

        public static void AddDbContexts
            <TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>
            (this IServiceCollection services, IHostingEnvironment hostingEnvironment,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions,
            Action<DbContextOptionsBuilder> logDbContextOptions)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
        {
            if (hostingEnvironment.IsStaging())
            {
                services.RegisterDbContextsStaging<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>();
            }
            else
            {
                services.RegisterDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(identityDbContextOptions, configurationStoreOptions, operationalStoreOptions, logDbContextOptions);
            }
        }

        /// <summary>
        /// Register in memory DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// For testing purpose only
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TLogDbContext"></typeparam>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <param name="services"></param>
        private static void RegisterDbContextsStaging
            <TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>
            (this IServiceCollection services)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
        {
            var persistedGrantsDatabaseName = Guid.NewGuid().ToString();
            var configurationDatabaseName = Guid.NewGuid().ToString();
            var logDatabaseName = Guid.NewGuid().ToString();
            var identityDatabaseName = Guid.NewGuid().ToString();

            var operationalStoreOptions = new OperationalStoreOptions();
            services.AddSingleton(operationalStoreOptions);

            var storeOptions = new ConfigurationStoreOptions();
            services.AddSingleton(storeOptions);

            services.AddDbContext<TIdentityDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(identityDatabaseName));
            services.AddDbContext<TPersistedGrantDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(persistedGrantsDatabaseName));
            services.AddDbContext<TConfigurationDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(configurationDatabaseName));
            services.AddDbContext<TLogDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(logDatabaseName));
        }

        /// <summary>
        /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// Configure the connection strings in AppSettings.json
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TLogDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void RegisterDbContexts
            <TAdminIdentityDbContext, TIdentityServerConfigurationDbContext, TIdentityServerPersistedGrantDbContext, TAdminLogDbContext>
            (this IServiceCollection services,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions,
            Action<DbContextOptionsBuilder> logDbContextOptions)
            where TAdminIdentityDbContext : DbContext
            where TIdentityServerConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TIdentityServerPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TAdminLogDbContext : DbContext, IAdminLogDbContext
        {
            services.AddDbContext<TAdminIdentityDbContext>(identityDbContextOptions);
            services.AddConfigurationDbContext<TIdentityServerConfigurationDbContext>(configurationStoreOptions);
            services.AddOperationalDbContext<TIdentityServerPersistedGrantDbContext>(operationalStoreOptions);
            services.AddDbContext<TAdminLogDbContext>(logDbContextOptions);
        }

        #endregion Add DbContexts

        public static ISkorubaIdentityServerAdminBuilder AddCustomIdentity
            <TIdentityDbContext, TUserIdentity, TUserIdentityRole>
            (this ISkorubaIdentityServerAdminBuilder builder, Action<IdentityOptions> identityOptions)
            where TIdentityDbContext : DbContext
            where TUserIdentity : class
            where TUserIdentityRole : class
        {
            builder.Services
                .AddIdentity<TUserIdentity, TUserIdentityRole>(identityOptions)
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddDefaultTokenProviders();

            return builder;
        }
    }
}