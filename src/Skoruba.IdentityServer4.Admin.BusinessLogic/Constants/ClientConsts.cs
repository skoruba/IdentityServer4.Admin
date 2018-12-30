using System.Collections.Generic;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Constants
{
    public static class ClientConsts
    {
        public static List<string> GetSecretTypes()
        {
            var secretTypes = new List<string>
            {
                "SharedSecret",
                "X509Thumbprint",
                "X509Name",
                "X509CertificateBase64"
            };

            return secretTypes;
        }

        public static List<string> GetStandardClaims()
        {
            //http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            var standardClaims = new List<string>
            {
                "name",
                "given_name",
                "family_name",
                "middle_name",
                "nickname",
                "preferred_username",
                "profile",
                "picture",
                "website",
                "gender",
                "birthdate",
                "zoneinfo",
                "locale",
                "address",
                "updated_at"
            };

            return standardClaims;
        }

        public static List<string> GetGrantTypes()
        {
            var allowedGrantypes = new List<string>
                {
                    "implicit",
                    "client_credentials",
                    "authorization_code",
                    "hybrid",
                    "password",
                    "urn:ietf:params:oauth:grant-type:device_code"
                };

            return allowedGrantypes;
        }

        public static List<SelectItem> GetProtocolTypes()
        {
            var protocolTypes = new List<SelectItem> { new SelectItem("oidc", "OpenID Connect") };

            return protocolTypes;
        }
    }
}
