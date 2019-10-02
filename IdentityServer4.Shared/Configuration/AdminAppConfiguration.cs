
using Skoruba.IdentityServer4.Shared.Configuration.Constants;
using Skoruba.IdentityServer4.Shared.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Shared.Configuration
{
    public class AdminAppConfiguration : IAdminAppConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "https://localhost:44320";
        public string IdentityAdminRedirectUri { get; set; } = "https://localhost:44320/signin-oidc";

        public string IdentityServerBaseUrl { get; set; } = "https://localhost:44350/";
        public string ClientId { get; set; } = AuthenticationConsts.OidcClientId;
        public string[] Scopes { get; set; }

        public string IdentityAdminApiSwaggerUIClientId { get; } = AuthenticationConsts.IdentityAdminApiSwaggerClientId;
        public string IdentityAdminApiSwaggerUIRedirectUrl { get; } = "https://localhost:5001/swagger/oauth2-redirect.html";
        public string IdentityAdminApiScope { get; } = AuthenticationConsts.IdentityAdminApiScope;

        public string ClientSecret { get; set; } = AuthenticationConsts.OidcClientSecret;
        public string OidcResponseType { get; set; } = AuthenticationConsts.OidcResponseType;


    }
}
