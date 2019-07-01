using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.STS.Identity.DependencyInjection
{
    public class Builder : IBuilder
    {
        public Builder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}