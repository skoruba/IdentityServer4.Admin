using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "https://localhost:44391";

        public string IdentityAdminRedirectUri { get; set; } = "https://localhost:44391/signin-oidc";

        public string IdentityServerBaseUrl { get; set; } = "https://localhost:44390";
    }
}
