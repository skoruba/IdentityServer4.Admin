using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.DependencyInjection
{
    public class SkorubaIdentityServerBuilder : ISkorubaIdentityServerBuilder
    {
        public SkorubaIdentityServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }

    public class SkorubaIdentityServerAdminBuilder : ISkorubaIdentityServerAdminBuilder
    {
        public SkorubaIdentityServerAdminBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}