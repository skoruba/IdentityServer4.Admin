using Microsoft.Extensions.Configuration;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static SkorubaTenantConfigurationBuilder AddSingleTenantConfiguration(this IServiceCollection services)
        {
            var configuration = new MultiTenantConfiguration() { MultiTenantEnabled = false };

            return services.AddMultiTenantConfiguration<SkorubaSingleTenantContext>(configuration);
        }

        public static SkorubaTenantConfigurationBuilder AddMultiTenantConfiguration<TSkorubaTenantContext>(this IServiceCollection services, MultiTenantConfiguration configuration)
            where TSkorubaTenantContext : class, ISkorubaTenantContext
        {
            services.AddSingleton(configuration);

            services.AddScoped<ISkorubaTenantContext, TSkorubaTenantContext>();

            if (configuration.MultiTenantEnabled)
            {
                services.AddScoped<ValidateTenantRequirement>();
            }

            return new SkorubaTenantConfigurationBuilder(services);

        }
        /// <summary>
        /// Adds validation for determining if the tenant must be resolved or not.  Typical implementation is to allow a null tenant for data access code where the tenant may not be available and also not necessary.
        /// </summary>
        /// <typeparam name="TValidator"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static SkorubaTenantConfigurationBuilder RegisterTenantIsRequiredValidation<TValidator>(this SkorubaTenantConfigurationBuilder builder)
        where TValidator : class, IValidateTenantRequirement
        {
            builder.Services.AddScoped<IValidateTenantRequirement, TValidator>();
            return builder;
        }

    }

}