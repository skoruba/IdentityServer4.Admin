using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SkorubaTenantConfigurationBuilder
    {
        public SkorubaTenantConfigurationBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}