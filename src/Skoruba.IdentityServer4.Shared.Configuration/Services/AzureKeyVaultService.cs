// Original file comes from: https://github.com/damienbod/IdentityServer4AspNetCoreIdentityTemplate
// Modified by Jan Škoruba

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Common;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services
{
    public class AzureKeyVaultService
    {
        private readonly AzureKeyVaultConfiguration _azureKeyVaultConfiguration;

        public AzureKeyVaultService(AzureKeyVaultConfiguration azureKeyVaultConfiguration)
        {
            if (azureKeyVaultConfiguration == null)
            {
                throw new ArgumentException("missing azureKeyVaultConfiguration");
            }

            if (string.IsNullOrEmpty(azureKeyVaultConfiguration.AzureKeyVaultEndpoint))
            {
                throw new ArgumentException("missing keyVaultEndpoint");
            }

            _azureKeyVaultConfiguration = azureKeyVaultConfiguration;
        }

        public async Task<(X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate)> GetCertificatesFromKeyVault()
        {
            (X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate) certs = (null, null);

            var keyVaultClient = BuildKeyVaultClient();

            var certificateItems = await GetAllEnabledCertificateVersionsAsync(keyVaultClient);
            var item = certificateItems.FirstOrDefault();
            if (item != null)
            {
                certs.ActiveCertificate = await GetCertificateAsync(item.Identifier.Identifier, keyVaultClient);
            }

            if (certificateItems.Count > 1)
            {
                certs.SecondaryCertificate = await GetCertificateAsync(certificateItems[1].Identifier.Identifier, keyVaultClient);
            }

            return certs;
        }

        /// <summary>
        /// Build KeyVaultClient according to authentication method
        /// </summary>
        /// <returns></returns>
        public IKeyVaultClient BuildKeyVaultClient()
        {
            IKeyVaultClient keyVaultClient;

            if (_azureKeyVaultConfiguration.UseClientCredentials)
            {
                keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
                {
                    var adCredential = new ClientCredential(_azureKeyVaultConfiguration.ClientId, _azureKeyVaultConfiguration.ClientSecret);
                    var authenticationContext = new AuthenticationContext(authority, null);
                    return (await authenticationContext.AcquireTokenAsync(resource, adCredential)).AccessToken;
                });
            }
            else
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            }

            return keyVaultClient;
        }

        private async Task<List<CertificateItem>> GetAllEnabledCertificateVersionsAsync(IKeyVaultClient keyVaultClient)
        {
            // Get all the certificate versions (this will also get the current active version)
            var certificateVersions = await keyVaultClient.GetCertificateVersionsAsync(_azureKeyVaultConfiguration.AzureKeyVaultEndpoint, _azureKeyVaultConfiguration.IdentityServerCertificateName);

            // Find all enabled versions of the certificate and sort them by creation date in descending order 
            return certificateVersions
              .Where(certVersion => certVersion.Attributes.Enabled.HasValue && certVersion.Attributes.Enabled.Value)
              .OrderByDescending(certVersion => certVersion.Attributes.Created)
              .ToList();
        }

        private async Task<X509Certificate2> GetCertificateAsync(string identifier, IKeyVaultClient keyVaultClient)
        {
            var certificateVersionBundle = await keyVaultClient.GetCertificateAsync(identifier);
            var certificatePrivateKeySecretBundle = await keyVaultClient.GetSecretAsync(certificateVersionBundle.SecretIdentifier.Identifier);
            var privateKeyBytes = Convert.FromBase64String(certificatePrivateKeySecretBundle.Value);
            var certificateWithPrivateKey = new X509Certificate2(privateKeyBytes, (string)null, X509KeyStorageFlags.MachineKeySet);

            return certificateWithPrivateKey;
        }
    }
}
