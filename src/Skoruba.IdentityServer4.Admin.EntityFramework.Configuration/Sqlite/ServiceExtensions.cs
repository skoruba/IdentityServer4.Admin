using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Storage;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.Configuration;

using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.Sqlite
{
    public static class DatabaseExtensions
    {
        public static void RegisterSqliteDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext, TAuditLog>(this IServiceCollection services, ConnectionStringsConfiguration connectionStrings, DatabaseMigrationsConfiguration databaseMigrations) where TIdentityDbContext : DbContext where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext where TLogDbContext : DbContext, IAdminLogDbContext where TAuditLoggingDbContext : DbContext, IAuditLoggingDbContext<TAuditLog> where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext where TAuditLog : AuditLog
        {
            string migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<TIdentityDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlite(connectionStrings.IdentityDbConnection, delegate (SqliteDbContextOptionsBuilder sql)
                {
                    sql.MigrationsAssembly(databaseMigrations.IdentityDbMigrationsAssembly ?? migrationsAssembly);
                });
            });
            services.AddConfigurationDbContext<TConfigurationDbContext>(delegate (ConfigurationStoreOptions options)
            {
                options.ConfigureDbContext = delegate (DbContextOptionsBuilder b)
                {
                    b.UseSqlite(connectionStrings.ConfigurationDbConnection, delegate (SqliteDbContextOptionsBuilder sql)
                    {
                        sql.MigrationsAssembly(databaseMigrations.ConfigurationDbMigrationsAssembly ?? migrationsAssembly);
                    });
                };
            });
            services.AddOperationalDbContext<TPersistedGrantDbContext>(delegate (OperationalStoreOptions options)
            {
                options.ConfigureDbContext = delegate (DbContextOptionsBuilder b)
                {
                    b.UseSqlite(connectionStrings.PersistedGrantDbConnection, delegate (SqliteDbContextOptionsBuilder sql)
                    {
                        sql.MigrationsAssembly(databaseMigrations.PersistedGrantDbMigrationsAssembly ?? migrationsAssembly);
                    });
                };
            });
            services.AddDbContext<TLogDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlite(connectionStrings.AdminLogDbConnection, delegate (SqliteDbContextOptionsBuilder optionsSql)
                {
                    optionsSql.MigrationsAssembly(databaseMigrations.AdminLogDbMigrationsAssembly ?? migrationsAssembly);
                });
            });
            services.AddDbContext<TAuditLoggingDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlite(connectionStrings.AdminAuditLogDbConnection, delegate (SqliteDbContextOptionsBuilder optionsSql)
                {
                    optionsSql.MigrationsAssembly(databaseMigrations.AdminAuditLogDbMigrationsAssembly ?? migrationsAssembly);
                });
            });
            if (string.IsNullOrEmpty(connectionStrings.DataProtectionDbConnection))
            {
                return;
            }

            services.AddDbContext<TDataProtectionDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlite(connectionStrings.DataProtectionDbConnection, delegate (SqliteDbContextOptionsBuilder optionsSql)
                {
                    optionsSql.MigrationsAssembly(databaseMigrations.DataProtectionDbMigrationsAssembly ?? migrationsAssembly);
                });
            });
        }

        public static void RegisterSqliteDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TDataProtectionDbContext>(this IServiceCollection services, string identityConnectionString, string configurationConnectionString, string persistedGrantConnectionString, string dataProtectionConnectionString) where TIdentityDbContext : DbContext where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            string migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<TIdentityDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlite(identityConnectionString, delegate (SqliteDbContextOptionsBuilder sql)
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                });
            });
            services.AddConfigurationDbContext<TConfigurationDbContext>(delegate (ConfigurationStoreOptions options)
            {
                options.ConfigureDbContext = delegate (DbContextOptionsBuilder b)
                {
                    b.UseSqlite(configurationConnectionString, delegate (SqliteDbContextOptionsBuilder sql)
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                    });
                };
            });
            services.AddOperationalDbContext<TPersistedGrantDbContext>(delegate (OperationalStoreOptions options)
            {
                options.ConfigureDbContext = delegate (DbContextOptionsBuilder b)
                {
                    b.UseSqlite(persistedGrantConnectionString, delegate (SqliteDbContextOptionsBuilder sql)
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                    });
                };
            });
            services.AddDbContext<TDataProtectionDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlite(dataProtectionConnectionString, delegate (SqliteDbContextOptionsBuilder sql)
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                });
            });
        }
    }
}
