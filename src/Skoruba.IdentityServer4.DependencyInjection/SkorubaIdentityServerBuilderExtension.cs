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

namespace Skoruba.IdentityServer4.DependencyInjection
{
    public static partial class SkorubaIdentityServerBuilderExtensions
    {
        #region Single Tenant Configuration

        public static ISkorubaIdentityServerBuilder AddSingleTenantConfiguration
            (this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILogger logger,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<IdentityOptions> identityOptions,
            Action<IdentityServerOptions> identityServerOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions)
        {
            var builder = services.AddIdentityConfiguration
                <AdminIdentityDbContext, UserIdentity, UserIdentityRole>
                (hostingEnvironment, identityDbContextOptions, identityOptions);

            builder.AddIdentityServer
                <UserIdentity, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext>
                (configuration, hostingEnvironment, logger, identityServerOptions, configurationStoreOptions, operationalStoreOptions);

            return builder;
        }

        #endregion Single Tenant Configuration

        #region Multi Tenant Configuration

        public static ISkorubaIdentityServerBuilder AddMultiTenantConfiguration
            (this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILogger logger,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<IdentityOptions> identityOptions,
            Action<IdentityServerOptions> identityServerOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions)
        {
            var builder = services.AddIdentityConfiguration
                <MultiTenantUserIdentityDbContext, MultiTenantUserIdentity, UserIdentityRole>
                (hostingEnvironment, identityDbContextOptions, identityOptions);

            builder.AddIdentityServer
                <MultiTenantUserIdentity, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext>
                (configuration, hostingEnvironment, logger, identityServerOptions, configurationStoreOptions, operationalStoreOptions);

            return builder;
        }

        #endregion Multi Tenant Configuration

        public static ISkorubaIdentityServerBuilder AddIdentityConfiguration
            <TContext, TIdentityUser, TIdentityUserRole>
            (this IServiceCollection services, IHostingEnvironment hostingEnvironment, Action<DbContextOptionsBuilder> dbContextOptions, Action<IdentityOptions> identityOptions)
            where TContext : DbContext
            where TIdentityUser : class
            where TIdentityUserRole : class
        {
            var builder = new SkorubaIdentityServerBuilder(services);
            if (hostingEnvironment.IsStaging())
            {
                builder.AddIdentityDbContextStaging<TContext>();
            }
            else
            {
                builder.AddIdentityDbContextProduction<TContext>(dbContextOptions);
            }

            builder.AddUserIdentity<TContext, TIdentityUser, TIdentityUserRole>(identityOptions);

            return builder;
        }

        private static ISkorubaIdentityServerBuilder AddIdentityDbContextStaging
            <TContext>
            (this ISkorubaIdentityServerBuilder builder)
             where TContext : DbContext
        {
            var identityDatabaseName = Guid.NewGuid().ToString();
            builder.Services.AddDbContext<TContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(identityDatabaseName));
            return builder;
        }

        private static ISkorubaIdentityServerBuilder AddIdentityDbContextProduction
            <TContext>
            (this ISkorubaIdentityServerBuilder builder, Action<DbContextOptionsBuilder> options)
             where TContext : DbContext
        {
            builder.Services.AddDbContext<TContext>(options);
            return builder;
        }

        private static ISkorubaIdentityServerBuilder AddUserIdentity
            <TIdentityDbContext, TUserIdentity, TUserIdentityRole>
            (this ISkorubaIdentityServerBuilder builder, Action<IdentityOptions> options)
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

        private static ISkorubaIdentityServerBuilder AddIdentityServer
            <TUserIdentity, TConfigurationDbContext, TPersistedGrantDbContext>
            (this ISkorubaIdentityServerBuilder builder,
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
                .AddIdentityServerStoresWithDbContexts<TConfigurationDbContext, TPersistedGrantDbContext>(configuration, hostingEnvironment, logger, configurationStoreOptions, operationalStoreOptions);

            return builder;
        }

        private static IIdentityServerBuilder AddIdentityServerStoresWithDbContexts
            <TConfigurationDbContext, TPersistedGrantDbContext>
            (this IIdentityServerBuilder builder, IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILogger logger,
            Action<ConfigurationStoreOptions> configurationStoreOptions, Action<OperationalStoreOptions> operationalStoreOptions)
            where TPersistedGrantDbContext : DbContext, IPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IConfigurationDbContext
        {
            if (hostingEnvironment.IsStaging())
            {
                return RegisterIdentityServerStoresWithDbContextsStaging<TConfigurationDbContext, TPersistedGrantDbContext>(builder);
            }
            else
            {
                return RegisterIdentityServerStoresWithDbContexts<TConfigurationDbContext, TPersistedGrantDbContext>(builder, configuration, logger, configurationStoreOptions, operationalStoreOptions);
            }
        }

        private static IIdentityServerBuilder RegisterIdentityServerStoresWithDbContextsStaging
            <TConfigurationDbContext, TPersistedGrantDbContext>
            (IIdentityServerBuilder builder)
            where TPersistedGrantDbContext : DbContext, IPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IConfigurationDbContext
        {
            var configurationDatabaseName = Guid.NewGuid().ToString();
            var operationalDatabaseName = Guid.NewGuid().ToString();

            builder.AddConfigurationStore<TConfigurationDbContext>(options =>
            {
                options.ConfigureDbContext = b => b.UseInMemoryDatabase(configurationDatabaseName);
            });

            builder.AddOperationalStore<TPersistedGrantDbContext>(options =>
            {
                options.ConfigureDbContext = b => b.UseInMemoryDatabase(operationalDatabaseName);
            });

            return builder;
        }

        private static IIdentityServerBuilder RegisterIdentityServerStoresWithDbContexts
            <TConfigurationDbContext, TPersistedGrantDbContext>
            (IIdentityServerBuilder builder, IConfiguration configuration, ILogger logger,
            Action<ConfigurationStoreOptions> configurationStoreOptions, Action<OperationalStoreOptions> operationalStoreOptions)
            where TPersistedGrantDbContext : DbContext, IPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IConfigurationDbContext
        {
            // Config DB from existing connection
            builder.AddConfigurationStore<TConfigurationDbContext>(configurationStoreOptions);

            // Operational DB from existing connection
            builder.AddOperationalStore<TPersistedGrantDbContext>(operationalStoreOptions);

            builder.AddCustomSigningCredential(configuration, logger);
            builder.AddCustomValidationKey(configuration, logger);

            return builder;
        }

        //public static ISkorubaIdentityServerBuilder AddUserIdentity
        //    <TUserIdentity, TUserIdentityRole, TIdentityDbContext, TSigninManager, TUserManager, TUserStore>
        //    (this ISkorubaIdentityServerBuilder builder, Action<IdentityOptions> options)
        //    where TIdentityDbContext : DbContext
        //    where TUserIdentity : class
        //    where TUserIdentityRole : class
        //    where TSigninManager : class
        //    where TUserManager : class
        //    where TUserStore : class
        //{
        //    builder.Services
        //        .AddIdentity<TUserIdentity, TUserIdentityRole>(options)
        //        .AddEntityFrameworkStores<TIdentityDbContext>()
        //        .AddUserStore<TUserStore>()
        //        .AddSignInManager<TSigninManager>()
        //        .AddUserManager<TUserManager>()
        //        .AddDefaultTokenProviders();

        //    return builder;
        //}
    }

    public static class IdentityServerBuilderExtensions
    {
        private const string CertificateNotFound = "Certificate not found";
        private const string SigningCertificateThumbprintNotFound = "Signing certificate thumbprint not found";
        private const string SigningCertificatePathIsNotSpecified = "Signing certificate file path is not specified";

        private const string ValidationCertificateThumbprintNotFound = "Validation certificate thumbprint not found";
        private const string ValidationCertificatePathIsNotSpecified = "Validation certificate file path is not specified";

        /// <summary>
        /// Add custom signing certificate from certification store according thumbprint or from file
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder, IConfiguration configuration, ILogger logger)
        {
            var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();

            if (certificateConfiguration.UseSigningCertificateThumbprint)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificateThumbprint))
                {
                    throw new Exception(SigningCertificateThumbprintNotFound);
                }

                var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadOnly);

                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.SigningCertificateThumbprint, true);
                if (certCollection.Count == 0)
                {
                    throw new Exception(CertificateNotFound);
                }

                var certificate = certCollection[0];

                builder.AddSigningCredential(certificate);
            }
            else if (certificateConfiguration.UseSigningCertificatePfxFile)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificatePfxFilePath))
                {
                    throw new Exception(SigningCertificatePathIsNotSpecified);
                }

                if (File.Exists(certificateConfiguration.SigningCertificatePfxFilePath))
                {
                    try
                    {
                        builder.AddSigningCredential(new X509Certificate2(certificateConfiguration.SigningCertificatePfxFilePath, certificateConfiguration.SigningCertificatePfxFilePassword));
                    }
                    catch (CryptographicException e)
                    {
                        logger.LogError($"There was an error adding the key file - during the creation of the signing key {e.Message}");
                    }
                }
                else
                {
                    throw new Exception($"Signing key file: {certificateConfiguration.SigningCertificatePfxFilePath} not found");
                }
            }
            else if (certificateConfiguration.UseTemporarySigningKeyForDevelopment)
            {
                builder.AddDeveloperSigningCredential();
            }

            return builder;
        }

        /// <summary>
        /// Add custom validation key for signing key rollover
        /// http://docs.identityserver.io/en/latest/topics/crypto.html#signing-key-rollover
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddCustomValidationKey(this IIdentityServerBuilder builder, IConfiguration configuration, ILogger logger)
        {
            var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();

            if (certificateConfiguration.UseValidationCertificateThumbprint)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificateThumbprint))
                {
                    throw new Exception(ValidationCertificateThumbprintNotFound);
                }

                var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadOnly);

                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.ValidationCertificateThumbprint, false);
                if (certCollection.Count == 0)
                {
                    throw new Exception(CertificateNotFound);
                }

                var certificate = certCollection[0];

                builder.AddValidationKey(certificate);
            }
            else if (certificateConfiguration.UseValidationCertificatePfxFile)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificatePfxFilePath))
                {
                    throw new Exception(ValidationCertificatePathIsNotSpecified);
                }

                if (File.Exists(certificateConfiguration.ValidationCertificatePfxFilePath))
                {
                    try
                    {
                        builder.AddValidationKey(new X509Certificate2(certificateConfiguration.ValidationCertificatePfxFilePath, certificateConfiguration.ValidationCertificatePfxFilePassword));
                    }
                    catch (CryptographicException e)
                    {
                        logger.LogError($"There was an error adding the key file - during the creation of the validation key {e.Message}");
                    }
                }
                else
                {
                    throw new Exception($"Validation key file: {certificateConfiguration.SigningCertificatePfxFilePath} not found");
                }
            }

            return builder;
        }
    }

    public class CertificateConfiguration
    {
        public bool UseTemporarySigningKeyForDevelopment { get; set; }

        public bool UseSigningCertificateThumbprint { get; set; }

        public string SigningCertificateThumbprint { get; set; }

        public bool UseSigningCertificatePfxFile { get; set; }

        public string SigningCertificatePfxFilePath { get; set; }

        public string SigningCertificatePfxFilePassword { get; set; }

        public bool UseValidationCertificateThumbprint { get; set; }

        public string ValidationCertificateThumbprint { get; set; }

        public bool UseValidationCertificatePfxFile { get; set; }

        public string ValidationCertificatePfxFilePath { get; set; }

        public string ValidationCertificatePfxFilePassword { get; set; }
    }
}