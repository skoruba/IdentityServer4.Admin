namespace SkorubaIdentityServer4Admin.Admin.Api.Configuration.Constants
{
    public class AuthorizationConsts
    {
        public const string IdentityServerBaseUrl = "http://localhost:5000";
        public const string OidcSwaggerUIClientId = "skoruba_identity_admin_api_swaggerui";
        public const string OidcApiName = "skoruba_identity_admin_api";

        public const string AdministrationPolicy = "RequireAdministratorRole";
        public const string AdministrationRole = "SkorubaIdentityAdminAdministrator";
    }
}