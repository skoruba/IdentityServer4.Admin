using System.Reflection;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Configuration;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.MySql.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// Configure the connection strings in AppSettings.json
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TLogDbContext"></typeparam>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <typeparam name="TAuditLoggingDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="identityConnectionString"></param>
        /// <param name="configurationConnectionString"></param>
        /// <param name="persistedGrantConnectionString"></param>
        /// <param name="errorLoggingConnectionString"></param>
        /// <param name="auditLoggingConnectionString"></param>
        public static void RegisterMySqlDbContexts<TIdentityDbContext, TConfigurationDbContext,
            TPersistedGrantDbContext, TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext>(this IServiceCollection services,
            ConnectionStringsConfiguration connectionStrings,
            DatabaseMigrationsConfiguration databaseMigrations)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLoggingDbContext : DbContext, IAuditLoggingDbContext<AuditLog>
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            // Config DB for identity
            services.AddDbContext<TIdentityDbContext>(options =>
                options.UseMySql(connectionStrings.IdentityDbConnection, sql => sql.MigrationsAssembly(databaseMigrations.IdentityDbMigrationsAssembly ?? migrationsAssembly)));

            // Config DB from existing connection
            services.AddConfigurationDbContext<TConfigurationDbContext>(options =>
                options.ConfigureDbContext = b =>
                    b.UseMySql(connectionStrings.ConfigurationDbConnection, sql => sql.MigrationsAssembly(databaseMigrations.ConfigurationDbMigrationsAssembly ?? migrationsAssembly)));

            // Operational DB from existing connection
            services.AddOperationalDbContext<TPersistedGrantDbContext>(options => options.ConfigureDbContext = b =>
                b.UseMySql(connectionStrings.PersistedGrantDbConnection, sql => sql.MigrationsAssembly(databaseMigrations.PersistedGrantDbMigrationsAssembly ?? migrationsAssembly)));

            // Log DB from existing connection
            services.AddDbContext<TLogDbContext>(options => options.UseMySql(connectionStrings.AdminLogDbConnection,
                optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.AdminLogDbMigrationsAssembly ?? migrationsAssembly)));

            // Audit logging connection
            services.AddDbContext<TAuditLoggingDbContext>(options => options.UseMySql(connectionStrings.AdminAuditLogDbConnection,
                optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.AdminAuditLogDbMigrationsAssembly ?? migrationsAssembly)));

            // DataProtectionKey DB from existing connection
            if(!string.IsNullOrEmpty(connectionStrings.DataProtectionDbConnection))
                services.AddDbContext<TDataProtectionDbContext>(options => options.UseMySql(connectionStrings.DataProtectionDbConnection,
                    optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DataProtectionDbMigrationsAssembly ?? migrationsAssembly)));
        }

        /// <summary>
        /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants and Identity
        /// Configure the connection strings in AppSettings.json
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="identityConnectionString"></param>
        /// <param name="configurationConnectionString"></param>
        /// <param name="persistedGrantConnectionString"></param>
        public static void RegisterMySqlDbContexts<TIdentityDbContext, TConfigurationDbContext,
            TPersistedGrantDbContext, TDataProtectionDbContext>(this IServiceCollection services,
            string identityConnectionString, string configurationConnectionString,
            string persistedGrantConnectionString, string dataProtectionConnectionString)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            // Config DB for identity
            services.AddDbContext<TIdentityDbContext>(options => options.UseMySql(identityConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            // Config DB from existing connection
            services.AddConfigurationDbContext<TConfigurationDbContext>(options => options.ConfigureDbContext = b => b.UseMySql(configurationConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            // Operational DB from existing connection
            services.AddOperationalDbContext<TPersistedGrantDbContext>(options => options.ConfigureDbContext = b => b.UseMySql(persistedGrantConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            // DataProtectionKey DB from existing connection
            services.AddDbContext<TDataProtectionDbContext>(options => options.UseMySql(dataProtectionConnectionString,
                optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly)));

        }
    }
}