using Microsoft.Extensions.Options;
using IdentityServer4.Shared.Configuration.Intefaces;
using IdentityServer4.Shared.Configuration.Interfaces;
using IdentityServer4.STS.Identity.Configuration;

namespace IdentityServer4.Shared.Configuration
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
