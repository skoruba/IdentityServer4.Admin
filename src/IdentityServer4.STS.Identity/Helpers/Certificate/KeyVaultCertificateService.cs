using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace IdentityServer4.STS.Identity.Helpers.Certificate
{
	public class KeyVaultCertificateService : ICertificateService
	{
		private readonly string _vaultAddress;
		private readonly string _vaultClientId;
		private readonly string _vaultClientSecret;
		private readonly ILogger _logger;

		public KeyVaultCertificateService(string vaultAddress, string vaultClientId, string vaultClientSecret, ILogger logger)
		{
			_vaultAddress = vaultAddress;
			_vaultClientId = vaultClientId;
			_vaultClientSecret = vaultClientSecret;
			_logger = logger;
		}

		public X509Certificate2 GetCertificateFromKeyVault(string vaultCertificateName)
		{
			var keyVaultClient = new KeyVaultClient(AuthenticationCallback);
			_logger.LogInformation("Vault Address: ", _vaultAddress);
			_logger.LogInformation("Vault Certificate Name: ", vaultCertificateName);

			var certBundle = keyVaultClient.GetCertificateAsync(vaultCertificateName).Result;	

			_logger.LogInformation("Cert Bundle :",certBundle.ToString());

			var certContent = keyVaultClient.GetSecretAsync(certBundle.SecretIdentifier.Identifier).Result;

			_logger.LogInformation("Cert Value : ",certContent.Value);

			var certBytes = Convert.FromBase64String(certContent.Value);
			var cert = new X509Certificate2(certBytes, string.Empty, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
			return cert;
		}

		private async Task<string> AuthenticationCallback(string authority, string resource, string scope)
		{
			var clientCredential = new ClientCredential(_vaultClientId, _vaultClientSecret);

			var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
			var result = await context.AcquireTokenAsync(resource, clientCredential);

			return result.AccessToken;
		}
	}
}