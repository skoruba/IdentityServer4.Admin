namespace Skoruba.IdentityServer4.Admin.Configuration.Constants
{
    public class AuthorizationConsts
    {
        public const string AdministrationPolicy = "RequireAdministratorRole";
        public const string AdministrationRole = "SkorubaIdentityAdminAdministrator";
        public const string TenantAdministratorPolicy = "RequireTenantAdministratorRole";
        public const string TenantAdministratorRole = "TenantAdministrator";
        public const string TenantAdvancedUserPolicy = "RequireTenantAdvancedUserRole";
        public const string TenantAdvancedUserRole = "TenantAdvancedUser";
    }
}