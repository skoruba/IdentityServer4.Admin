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
        // Default implementation
        public static ISkorubaIdentityServerAdminBuilder AddSingleTenantConfiguration
            (this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            ILogger logger,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions,
            Action<DbContextOptionsBuilder> logDbContextOptions)
        {
            var builder = new SkorubaIdentityServerAdminBuilder(services);

            builder.Services.AddCustomConfiguration
                <AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>
                (configuration, hostingEnvironment, logger, identityDbContextOptions, configurationStoreOptions, operationalStoreOptions, logDbContextOptions);

            return builder;
        }

        public static ISkorubaIdentityServerAdminBuilder AddSingleTenantIdentity
            (this ISkorubaIdentityServerAdminBuilder builder, Action<IdentityOptions> identityOptions)
        {
            builder.AddCustomIdentity<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(identityOptions);

            return builder;
        }
    }
}