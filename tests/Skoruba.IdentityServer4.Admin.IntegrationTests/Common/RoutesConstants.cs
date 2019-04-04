using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.IntegrationTests.Common
{
    public static class RoutesConstants
    {
        public static List<string> GetConfigureRoutes()
        {
            var configureRoutes = new List<string>
            {
                "Clients",
                "Client/1",
                "ClientClone/1",
                "ClientDelete/1",
                "ClientClaims/1",
                "ClientProperties/1",
                "ClientClaimDelete/1",
                "ClientPropertyDelete/1",
                "ClientSecrets/1",
                "ClientSecretDelete/1",
                "IdentityResourceDelete/1",
                "IdentityResources",
                "IdentityResource/1",
                "ApiResource/1",
                "ApiSecrets/1",
                "ApiScopes/1",
                "ApiScopeDelete/1?scope=1",
                "ApiResourceDelete/1",
                "ApiResources",
                "ApiSecretDelete/1"
            };

            return configureRoutes;
        }

        public static List<string> GetIdentityRoutes()
        {
            var identityRoutes = new List<string>
            {
                "Roles",
                "Role/1",
                "Users",
                "UserProfile/1",
                "UserRoles/1",
                "UserRolesDelete/1?roleId=1",
                "UserClaims/1",
                "UserClaimsDelete/1?claimId=1",
                "UserProviders/1",
                "UserProvidersDelete/1?providerKey=facebook",
                "UserChangePassword/1",
                "RoleClaims/1",
                "RoleClaimsDelete/1?claimId=1",
                "RoleDelete/1",
                "UserDelete/1"
            };

            return identityRoutes;
        }

        public static List<string> GetGrantRoutes()
        {
            var grantRoutes = new List<string>
            {
                "PersistedGrants",
                "PersistedGrantDelete/1",
                "PersistedGrant/1"
            };

            return grantRoutes;
        }
    }
}