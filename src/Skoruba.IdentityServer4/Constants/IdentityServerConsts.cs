namespace Skoruba.IdentityServer4.Constants
{
    public class IdentityServerConsts
    {
        public const string AdministrationRole = "SkorubaIdentityAdminAdministrator";

#if DEBUG
        public const string IdentityAdminBaseUrl = "http://localhost:9000";
#else
		public const string IdentityAdminBaseUrl = "https://admin.skoruba.com";
#endif        
        public const string OidcClientId = "skoruba_identity_admin";
        public const string OidcClientName = "Skoruba Identity Admin";
    }
}