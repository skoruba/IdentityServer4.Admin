using SkorubaIdentityServer4Admin.STS.Identity.Configuration.Interfaces;

namespace SkorubaIdentityServer4Admin.STS.Identity.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {      
        public AdminConfiguration AdminConfiguration { get; } = new AdminConfiguration();
        public RegisterConfiguration RegisterConfiguration { get; } = new RegisterConfiguration();
    }
}





