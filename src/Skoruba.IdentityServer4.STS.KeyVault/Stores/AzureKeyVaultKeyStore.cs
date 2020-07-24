using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Tokens;
using Skoruba.IdentityServer4.STS.KeyVault.Configuration;

namespace Skoruba.IdentityServer4.STS.KeyVault.Stores
{
    public class AzureKeyVaultKeyStore : ISigningCredentialStore, IValidationKeysStore
    {
        private const string SigningAlgorithm = "PS256";
        
        private readonly AzureKeyVaultConfiguration _configuration;
        private readonly IKeyVaultClient _keyVaultClient;

        public AzureKeyVaultKeyStore(
            AzureKeyVaultConfiguration configuration,
            IKeyVaultClient keyVaultClient)
        {
            _configuration = configuration;
            _keyVaultClient = keyVaultClient;
        }

        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var response = await _keyVaultClient.GetKeyAsync(_configuration.KeyIdentifier);
            var key = new RsaSecurityKey(response.Key.ToRSA())
            {
                KeyId = response.KeyIdentifier.Version
            };
            
            return new SigningCredentials(key, SigningAlgorithm);
        }

        public async Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            var validationKeys = new List<SecurityKeyInfo>();
            var keyItemPage = await _keyVaultClient.GetKeyVersionsAsync(_configuration.KeyVaultUri, _configuration.KeyName);
            
            while (true)
            {
                var validKeys = keyItemPage.Where(key =>
                    key.Attributes?.Enabled == true &&
                    key.Attributes?.Expires > DateTime.UtcNow);
                
                foreach (var keyItem in validKeys)
                {
                    var keyBundle = await _keyVaultClient.GetKeyAsync(keyItem.Identifier.Identifier);
                    var key = new RsaSecurityKey(keyBundle.Key.ToRSA())
                    {
                        KeyId = keyBundle.KeyIdentifier.Version
                    };
                    
                    validationKeys.Add(new SecurityKeyInfo
                    {
                        Key = key,
                        SigningAlgorithm = SigningAlgorithm
                    });
                }

                if (keyItemPage.NextPageLink == null)
                {
                    break;
                }
                
                keyItemPage = await _keyVaultClient.GetKeyVersionsNextAsync(keyItemPage.NextPageLink);
            }

            return validationKeys;
        }
    }
}
