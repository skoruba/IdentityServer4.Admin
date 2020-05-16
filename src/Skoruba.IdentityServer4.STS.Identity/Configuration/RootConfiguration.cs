using Skoruba.IdentityServer4.STS.Identity.Configuration.Interfaces;
using Skoruba.MultiTenant.Configuration;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {      
        public AdminConfiguration AdminConfiguration { get; } = new AdminConfiguration();
        public RegisterConfiguration RegisterConfiguration { get; } = new RegisterConfiguration();
        public MultiTenantConfiguration MultiTenantConfiguration { get; set; } = new MultiTenantConfiguration();
    }
}