namespace IdentityServer4.Admin.Api.Configuration.Constants
{
    public class AuthorizationConsts
    {
        public const string IdentityServerBaseUrl = "https://localhost:44350";
        public const string OidcSwaggerUIClientId = "identity_admin_api_swaggerui";
        public const string OidcApiName = "identity_admin_api";

        public const string AdministrationPolicy = "RequireAdministratorRole";
        public const string AdministrationRole = "IdentityAdminAdministrator";
    }
}