using System.Collections.Generic;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Constants
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
