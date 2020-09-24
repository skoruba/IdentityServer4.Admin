using Skoruba.IdentityServer4.Shared.Configuration.Identity;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {      
        public AdminConfiguration AdminConfiguration { get; } = new AdminConfiguration();
        public RegisterConfiguration RegisterConfiguration { get; } = new RegisterConfiguration();
    }
}