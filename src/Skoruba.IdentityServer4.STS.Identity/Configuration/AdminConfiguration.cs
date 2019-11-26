using Skoruba.IdentityServer4.STS.Identity.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; }
        public string AdministrationRole { get; set; }
    }
}