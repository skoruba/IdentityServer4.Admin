using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; }

        public string IdentityAdminRedirectUri { get; set; }
        public string[] Scopes { get; set; }

        public string IdentityAdminApiSwaggerUIClientId { get; } = AuthenticationConsts.IdentityAdminApiSwaggerClientId;
        public string IdentityAdminApiSwaggerUIRedirectUrl { get; } = "http://localhost:5001/swagger/oauth2-redirect.html";
        public string IdentityAdminApiScope { get; } = AuthenticationConsts.IdentityAdminApiScope;


        public string IdentityServerBaseUrl { get; set; }
        public string ClientId { get; set; } 
        public string[] Scopes { get; set; }
        public string ClientSecret { get; set; }
        public string OidcResponseType { get; set; } = AuthenticationConsts.OidcAuthenticationScheme;
        public string AdministrationRole { get; set; }

    }
}
