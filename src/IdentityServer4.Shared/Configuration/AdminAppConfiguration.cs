
using IdentityServer4.Shared.Configuration.Constants;
using IdentityServer4.Shared.Configuration.Interfaces;

namespace IdentityServer4.Shared.Configuration
{
    public class AdminAppConfiguration : IAdminAppConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "https://securityadmin.azurewebsites.net";
        public string IdentityAdminRedirectUri { get; set; } = "https://securityadmin.azurewebsites.net/signin-oidc";

        public string IdentityServerBaseUrl { get; set; } = "https://is4sts.azurewebsites.net";
        public string ClientId { get; set; } = AuthenticationConsts.OidcClientId;
        public string[] Scopes { get; set; }

        public string IdentityAdminApiSwaggerUIClientId { get; } = AuthenticationConsts.IdentityAdminApiSwaggerClientId;
        public string IdentityAdminApiSwaggerUIRedirectUrl { get; } = "https://is4api.azurewebsites.net/swagger/oauth2-redirect.html";
        public string IdentityAdminApiScope { get; } = AuthenticationConsts.IdentityAdminApiScope;

        public string ClientSecret { get; set; } = AuthenticationConsts.OidcClientSecret;
        public string OidcResponseType { get; set; } = AuthenticationConsts.OidcResponseType;


    }
}