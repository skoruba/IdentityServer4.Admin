using System.Diagnostics;

namespace Skoruba.IdentityServer4.Shared.Configuration.Common
{
    public class AzureKeyVaultConfiguration
    {
        public string AzureKeyVaultEndpoint { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public bool UseClientCredentials { get; set; }

        public string IdentityServerCertificateName { get; set; }

        public string DataProtectionKeyIdentifier { get; set; }
    }
}