using Skoruba.IdentityServer4.Shared.Configuration.Intefaces;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class RegisterConfiguration : IRegisterConfiguration
    {
        public bool Enabled { get; set; } = true;
    }
}
