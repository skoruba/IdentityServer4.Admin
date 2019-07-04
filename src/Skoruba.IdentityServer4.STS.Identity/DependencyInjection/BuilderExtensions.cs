using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.IO;
using Skoruba.IdentityServer4.STS.Identity.DependencyInjection;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skoruba.IdentityServer4.STS.Identity.Configuration.ApplicationParts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Builder;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Intefaces;

//TODO: Need to rethink and refactor builder extensions for each web project
namespace Skoruba.IdentityServer4.STS.Identity.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring IdentityServer4 and AspNetIdentity
    /// with <see cref="Skoruba.IdentityServer4.STS.Identity"/>.
    /// </summary>
    public static partial class BuilderExtensions
    {
        #region Identity Configuration

        /// <summary>
        /// Registers the Identity DbContext
        /// </summary>
        /// <typeparam name="TAdminIdentityDbContext"></typeparam>
        /// <typeparam name="TUserIdentity"></typeparam>
        /// <typeparam name="TUserIdentityRole"></typeparam>
        /// <param name="services"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="identityDbContextOptions"></param>
        /// <returns></returns>
        public static IBuilder AddCustomConfiguration
            <TAdminIdentityDbContext, TUserIdentity, TUserIdentityRole>
            (this IServiceCollection services, IHostingEnvironment hostingEnvironment,
            Action<DbContextOptionsBuilder> identityDbContextOptions)
            where TAdminIdentityDbContext : DbContext
            where TUserIdentity : class
            where TUserIdentityRole : class
        {
            var builder = new Builder(services);

            builder.AddIdentityConfiguration
                <TAdminIdentityDbContext, TUserIdentity, TUserIdentityRole>
                (hostingEnvironment, identityDbContextOptions);

            builder.Services.AddScoped<UserResolver<TUserIdentity>>();

            return builder;
        }

        private static IBuilder AddIdentityConfiguration
            <TContext, TIdentityUser, TIdentityUserRole>
            (this IBuilder builder, IHostingEnvironment hostingEnvironment, Action<DbContextOptionsBuilder> dbContextOptions)
            where TContext : DbContext
            where TIdentityUser : class
            where TIdentityUserRole : class
        {
            if (hostingEnvironment.IsStaging())
            {
                builder.AddIdentityDbContextStaging<TContext>();
            }
            else
            {
                builder.AddIdentityDbContextProduction<TContext>(dbContextOptions);
            }

            return builder;
        }

        private static IBuilder AddIdentityDbContextStaging
            <TContext>
            (this IBuilder builder)
             where TContext : DbContext
        {
            var identityDatabaseName = Guid.NewGuid().ToString();
            builder.Services.AddDbContext<TContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(identityDatabaseName));
            return builder;
        }

        private static IBuilder AddIdentityDbContextProduction
            <TContext>
            (this IBuilder builder, Action<DbContextOptionsBuilder> options)
             where TContext : DbContext
        {
            builder.Services.AddDbContext<TContext>(options);
            return builder;
        }

        #endregion Identity Configuration

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
        public static IBuilder AddIdentityServer
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
        private static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder, IConfiguration configuration, ILogger logger)
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
        private static IIdentityServerBuilder AddCustomValidationKey(this IIdentityServerBuilder builder, IConfiguration configuration, ILogger logger)
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

        /// <summary>
        /// Register services for MVC and localization including available languages
        /// </summary>
        /// <param name="services"></param>
        public static IBuilder AddMvcWithLocalization<TUser, TKey>(this IBuilder builder)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            builder.Services.AddLocalization(opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; });

            builder.Services.TryAddTransient(typeof(IGenericControllerLocalizer<>), typeof(GenericControllerLocalizer<>));

            builder.Services.AddMvc(o =>
            {
                o.Conventions.Add(new GenericControllerRouteConvention());
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; })
                .AddDataAnnotationsLocalization()
                .ConfigureApplicationPartManager(m =>
                {
                    m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider<TUser, TKey>());
                });

            builder.Services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new[]
                    {
                        new CultureInfo("en"),
                        new CultureInfo("fa"),
                        new CultureInfo("ru"),
                        new CultureInfo("sv"),
                        new CultureInfo("zh")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en");
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                });
            return builder;
        }

        public static IBuilder AddUserValidator<TUser, TValidator>(this IBuilder builder)
            where TUser : class
            where TValidator : class
        {
            builder.Services.AddScoped(typeof(IUserValidator<TUser>), typeof(TValidator));
            return builder;
        }
    }
}