using SkorubaIdentityServer4Admin.STS.Identity.Configuration.Intefaces;

namespace SkorubaIdentityServer4Admin.STS.Identity.Configuration
{
    public class RegisterConfiguration : IRegisterConfiguration
    {
        public bool Enabled { get; set; } = true;
    }
}
