using SkorubaIdentityServer4Admin.Admin.Configuration.Constants;
using SkorubaIdentityServer4Admin.Admin.Configuration.Interfaces;

namespace SkorubaIdentityServer4Admin.Admin.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "http://localhost:9000";
        public string IdentityAdminRedirectUri { get; set; } = "http://localhost:9000/signin-oidc";

        public string IdentityServerBaseUrl { get; set; } = "http://localhost:5000";
        public string ClientId { get; set; } = AuthenticationConsts.OidcClientId;
        public string[] Scopes { get; set; }

        public string IdentityAdminApiSwaggerUIClientId { get; } = AuthenticationConsts.IdentityAdminApiSwaggerClientId;
        public string IdentityAdminApiSwaggerUIRedirectUrl { get; } = "http://localhost:5001/swagger/oauth2-redirect.html";
        public string IdentityAdminApiScope { get; } = AuthenticationConsts.IdentityAdminApiScope;

        public string ClientSecret { get; set; } = AuthenticationConsts.OidcClientSecret;
        public string OidcResponseType { get; set; } = AuthenticationConsts.OidcResponseType;


    }
}
