using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Intefaces;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {      
        public IAdminAppConfiguration AdminAppConfiguration { get; set; }
        public IRegisterConfiguration RegisterConfiguration { get; }

        public RootConfiguration(IOptions<AdminAppConfiguration> adminConfiguration, IOptions<RegisterConfiguration> registerConfiguration)
        {
            RegisterConfiguration = registerConfiguration.Value;
            AdminAppConfiguration = adminConfiguration.Value;
        }
    }
}