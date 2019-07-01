using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.Admin.Api.DependencyInjection
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