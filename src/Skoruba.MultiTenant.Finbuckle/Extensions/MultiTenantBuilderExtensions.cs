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
    public static class MultiTenantBuilderExtensions
    {
        /// <summary>
        /// Configure Finbuckle.MultiTenant services for the application.
        /// </summary>
        /// <param name="services">The IServiceCollection<c/> instance the extension method applies to.</param>
        /// <param name="isMultiTenant">Sets the <see cref="Skoruba.MultiTenant.Finbuckle.Configuration.Configuration"/> service IsMultiTenant value.</param>
        /// <returns>An new instance of MultiTenantBuilder.</returns>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder AddMultiTenant(this IServiceCollection services, bool isMultiTenant)
        {
            var builder = isMultiTenant
                ? services.AddMultiTenant()
                : new FinbuckleMultiTenantBuilder(services);

            var data = new Skoruba.MultiTenant.Finbuckle.Configuration.Configuration();
            builder.Services.AddSingleton(data);
            builder.Services.AddScoped<ISkorubaTenant, SkorubaTenant>();
            return builder;

        }

        /// <summary>
        /// Adds <see cref="Skoruba.MultiTenant.Finbuckle.Configuration.Configuration"/> service with values from the configuration section provided.  IsMultiTenant is set to true.
        /// </summary>
        /// <param name="builder">The MultiTenantBuilder<c/> instance the extension method applies to.</param>
        /// <returns>An new instance of MultiTenantBuilder.</returns>
        public static FinbuckleMultiTenantBuilder RegisterConfiguration(this FinbuckleMultiTenantBuilder builder, IConfigurationSection configurationSection)
        {
            builder.Services.Remove(builder.Services.FirstOrDefault(d => d.ServiceType == typeof(Skoruba.MultiTenant.Finbuckle.Configuration.Configuration)));

            var data = new Skoruba.MultiTenant.Finbuckle.Configuration.Configuration();
            configurationSection.Bind(data);
            builder.Services.AddSingleton(data);
            builder.Services.AddScoped<ISkorubaTenant, SkorubaTenant>();
            return builder;
        }

        /// <summary>
        /// Adds validation for determining if the tenant must be resolved or not.  Typical implementation is to allow a null tenant for data access code where the tenant may not be available and also not necessary.
        /// </summary>
        /// <typeparam name="TValidator"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static FinbuckleMultiTenantBuilder RegisterTenantIsRequiredValidation<TValidator>(this FinbuckleMultiTenantBuilder builder)
        where TValidator : class, IValidateTenantRequirement
        {
            builder.Services.TryAddScoped<ValidateTenantRequirement>();
            builder.Services.AddScoped<IValidateTenantRequirement, TValidator>();
            return builder;
        }
    }
}