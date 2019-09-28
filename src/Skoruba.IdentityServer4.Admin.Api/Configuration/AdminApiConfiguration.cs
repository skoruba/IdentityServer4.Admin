using Skoruba.IdentityServer4.Admin.Api.Configuration.Constants;

namespace Skoruba.IdentityServer4.Admin.Api.Configuration
{
    public class AdminApiConfiguration
    {
        public string IdentityServerBaseUrl { get; set; } = AuthorizationConsts.IdentityServerBaseUrl;

        public string OidcSwaggerUIClientId { get; set; } = AuthorizationConsts.OidcSwaggerUIClientId;

        public string OidcApiName { get; set; } = AuthorizationConsts.OidcApiName;
        public bool IdentityRequireHttpsMetadata { get; set; } = true;
    }
}