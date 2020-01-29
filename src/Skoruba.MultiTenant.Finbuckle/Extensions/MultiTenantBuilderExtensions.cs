using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Skoruba.MultiTenant;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Finbuckle;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds <see cref="Skoruba.MultiTenant.Finbuckle.Configuration.Configuration"/> services.
        /// </summary>
        /// <param name="builder">The SkorubaTenantConfigurationBuilder<c/> instance the extension method applies to.</param>
        /// <returns>An new instance of Finbuckle MultiTenantBuilder.</returns>
        public static FinbuckleMultiTenantBuilder WithFinbuckleMultiTenant(this SkorubaTenantConfigurationBuilder builder)
        {
            var finbuckleBuilder = builder.Services.AddMultiTenant();

            var data = new Skoruba.MultiTenant.Finbuckle.Configuration.Configuration();
            builder.Services.AddSingleton(data);

            return finbuckleBuilder;
        }

        /// <summary>
        /// Adds <see cref="Skoruba.MultiTenant.Finbuckle.Configuration.Configuration"/> services with optional <see cref="Skoruba.MultiTenant.Finbuckle.Configuration.Configuration"/> configuration values.
        /// </summary>
        /// <param name="builder">The SkorubaTenantConfigurationBuilder<c/> instance the extension method applies to.</param>
        /// <returns>An new instance of Finbuckle MultiTenantBuilder.</returns>
        public static FinbuckleMultiTenantBuilder WithFinbuckleMultiTenant(this SkorubaTenantConfigurationBuilder builder, IConfigurationSection configurationSection)
        {
            var finbuckleBuilder = builder.Services.AddMultiTenant();

            var data = new Skoruba.MultiTenant.Finbuckle.Configuration.Configuration();
            configurationSection.Bind(data);
            builder.Services.AddSingleton(data);

            return finbuckleBuilder;
        }
    }
}