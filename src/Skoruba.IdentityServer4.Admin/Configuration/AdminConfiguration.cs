using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminRedirectUri { get; set; }
        public string[] Scopes { get; set; }
        public string AdministrationRole { get; set; }
        public string IdentityServerBaseUrl { get; set; }
        public string ClientId { get; set; } 
        public string ClientSecret { get; set; }
        public string OidcResponseType { get; set; }
    }
}
