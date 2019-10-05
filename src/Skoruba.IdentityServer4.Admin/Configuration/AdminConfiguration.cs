using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; }

        public string IdentityAdminRedirectUri { get; set; }
        public string[] Scopes { get; set; }

        public string IdentityAdminApiSwaggerUIClientId { get; }
        public string IdentityAdminApiSwaggerUIRedirectUrl { get; }
        public string IdentityAdminApiScope { get; }
        public string AdministrationRole { get; }


        public string IdentityServerBaseUrl { get; set; }
        public string ClientId { get; set; } 
        public string ClientSecret { get; set; }
        public string OidcResponseType { get; set; } = AuthenticationConsts.OidcAuthenticationScheme;
    }
}
