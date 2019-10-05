namespace Skoruba.IdentityServer4.Admin.Api.Configuration
{
    public class AdminApiConfiguration
    {
        public string ApiName { get; set; }

        public string ApiVersion { get; set; }

        public string IdentityServerBaseUrl { get; set; }

        public string OidcSwaggerUIClientId { get; set; }

        public string OidcApiName { get; set; }

        public string AdministrationRole { get; set; }
    }
}