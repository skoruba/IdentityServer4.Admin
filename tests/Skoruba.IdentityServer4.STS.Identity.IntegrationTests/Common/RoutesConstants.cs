using System.Collections.Generic;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common
{
    public static class RoutesConstants
    {
        public static List<string> GetManageRoutes()
        {
            var manageRoutes = new List<string>
            {
                "Index",
                "ChangePassword",
                "PersonalData",
                "DeletePersonalData",
                "ExternalLogins",
                "TwoFactorAuthentication",
                "ResetAuthenticatorWarning",
                "EnableAuthenticator"
            };

            return manageRoutes;
        }
    }
}