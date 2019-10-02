using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.Shared.Configuration.Intefaces;
using Skoruba.IdentityServer4.Shared.Configuration.Interfaces;
using Skoruba.IdentityServer4.STS.Identity.Configuration;

namespace Skoruba.IdentityServer4.Shared.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {
        public IAdminAppConfiguration AdminAppConfiguration { get; set; }

        public IRegisterConfiguration RegisterConfiguration { get; }

        public RootConfiguration(IOptions<AdminAppConfiguration> adminConfiguration)
        {
            AdminAppConfiguration = adminConfiguration.Value;
        }        

        public RootConfiguration(IOptions<AdminAppConfiguration> adminConfiguration, IOptions<RegisterConfiguration> registerConfiguration)
        {
            RegisterConfiguration = registerConfiguration.Value;
            AdminAppConfiguration = adminConfiguration.Value;
        }
    }
}
