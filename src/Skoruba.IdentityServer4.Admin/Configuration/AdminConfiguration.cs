using Skoruba.IdentityServer4.Admin.Configuration.Constants;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class AdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "http://localhost:9000";

        public string IdentityAdminRedirectUri { get; set; } = "http://localhost:9000/signin-oidc";

        public string IdentityServerBaseUrl { get; set; } = "http://localhost:5000";

        public string ClientId { get; set; } = AuthenticationConsts.OidcClientId;

		public string[] Scopes { get; set; } = AuthenticationConsts.Scopes.ToArray();

		public string ClientSecret { get; set; } = AuthenticationConsts.OidcClientSecret;

		public string OidcResponseType { get; set; } = AuthenticationConsts.OidcResponseType;


    }
}
