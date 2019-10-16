using IdentityServer4.Shared.Configuration.Intefaces;

namespace IdentityServer4.STS.Identity.Configuration
{
    public class RegisterConfiguration : IRegisterConfiguration
    {
        public bool Enabled { get; set; } = true;
    }
}
