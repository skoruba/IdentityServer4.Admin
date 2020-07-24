using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.STS.KeyVault.Configuration;

namespace Skoruba.IdentityServer4.STS.KeyVault.Tokens
{
    public class KeyVaultTokenCreationService : DefaultTokenCreationService
    {
        private readonly AzureKeyVaultConfiguration _configuration;
        private readonly IKeyVaultClient _keyVaultClient;

        public KeyVaultTokenCreationService(
            AzureKeyVaultConfiguration configuration,
            ISystemClock clock,
            IKeyMaterialService keys,
            IdentityServerOptions options,
            ILogger<DefaultTokenCreationService> logger,
            IKeyVaultClient keyVaultClient) 
            : base(clock, keys, options, logger)
        {
            _configuration = configuration;
            _keyVaultClient = keyVaultClient;
        }

        protected override async Task<string> CreateJwtAsync(JwtSecurityToken jwt)
        {
            var plaintext = $"{jwt.EncodedHeader}.{jwt.EncodedPayload}";
            
            using var hasher = CryptoHelper.GetHashAlgorithmForSigningAlgorithm(jwt.SignatureAlgorithm);
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
            
            var response = await _keyVaultClient.SignAsync(_configuration.KeyIdentifier, jwt.SignatureAlgorithm, hash);

            return $"{plaintext}.{Base64UrlTextEncoder.Encode(response.Result)}";
        }
    }
}
