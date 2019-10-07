using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {
        public IAdminConfiguration AdminConfiguration { get; set; }
        public IIdentityDataConfiguration IdentityDataConfiguration { get; set; }
        public IIdentityServerDataConfiguration IdentityServerDataConfiguration { get; set; }

        public RootConfiguration(IOptions<AdminConfiguration> adminConfiguration, IOptions<IdentityServerDataConfiguration> clientConfiguration,
            IOptions<IdentityDataConfiguration> userConfiguration)
        {
            AdminConfiguration = adminConfiguration.Value;
            IdentityDataConfiguration = userConfiguration.Value;
            IdentityServerDataConfiguration = clientConfiguration.Value;
        }
    }
}
