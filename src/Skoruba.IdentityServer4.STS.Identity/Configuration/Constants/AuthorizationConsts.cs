namespace Skoruba.IdentityServer4.STS.Identity.Configuration.Constants
{
    public class AuthorizationConsts
    {
        public const string AdministrationPolicy = "RequireAdministratorRole";
        public const string AdministrationRole = "SkorubaIdentityAdminAdministrator";
        public const string Can2faBeDisabledPolicy = "Is2faRequiredPolicy";
        public const string TenantManagerPolicy = "RequireTenantManagerRole";
        public const string TenantManagerRole = "SkorubaIdentityTenantManager";
        public const string TenantAdministratorPolicy = "RequireTenantAdministratorRole";
        public const string TenantAdministratorRole = "TenantAdministrator";
        public const string TenantAdvancedUserPolicy = "RequireTenantAdvancedUserRole";
        public const string TenantAdvancedUserRole = "TenantAdvancedUser";
    }
}