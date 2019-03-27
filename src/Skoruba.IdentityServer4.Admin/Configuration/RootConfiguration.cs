using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {
        public IAdminConfiguration AdminConfiguration { get; set; }
        public IUserDataConfiguration UserDataConfiguration { get; set; }
        public IClientDataConfiguration ClientDataConfiguration { get; set; }

        public RootConfiguration(IOptions<AdminConfiguration> adminConfiguration, IOptions<ClientDataConfiguration> clientConfiguration,
            IOptions<UserDataConfiguration> userConfiguration)
        {
            AdminConfiguration = adminConfiguration.Value;
            UserDataConfiguration = userConfiguration.Value;
            ClientDataConfiguration = clientConfiguration.Value;
        }
    }
}
