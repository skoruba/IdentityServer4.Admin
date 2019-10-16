﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IdentityServer4.STS.Identity.Configuration;
using IdentityServer4.STS.Identity.Helpers.Certificate;

namespace IdentityServer4.STS.Identity.Helpers
{
	public static class IdentityServerBuilderExtensions
	{
		private const string CertificateNotFound = "Certificate not found";
		private const string SigningCertificateThumbprintNotFound = "Signing certificate thumbprint not found";
		private const string SigningCertificatePathIsNotSpecified = "Signing certificate file path is not specified";

		private const string ValidationCertificateThumbprintNotFound = "Validation certificate thumbprint not found";
		private const string ValidationCertificatePathIsNotSpecified = "Validation certificate file path is not specified";

		/// <summary>
		/// Add custom signing certificate from certification store according thumbprint or from file
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configuration"></param>
		/// <param name="logger"></param>
		/// <returns></returns>
		public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder, IConfiguration configuration, ILogger logger)
		{
			var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();

			if (certificateConfiguration.UseSigningCertificateThumbprint)
			{
				if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificateThumbprint))
				{
					throw new Exception(SigningCertificateThumbprintNotFound);
				}

				var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				certStore.Open(OpenFlags.ReadOnly);

				var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.SigningCertificateThumbprint, true);
				if (certCollection.Count == 0)
				{
					throw new Exception(CertificateNotFound);
				}

				var certificate = certCollection[0];

				builder.AddSigningCredential(certificate);
			}
			else if (certificateConfiguration.UseSigningCertificatePfxFile)
			{
				if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificatePfxFilePath))
				{
					throw new Exception(SigningCertificatePathIsNotSpecified);
				}

				if (File.Exists(certificateConfiguration.SigningCertificatePfxFilePath))
				{
					logger.LogInformation("inside signing certificate ");
					try
					{
						builder.AddSigningCredential(new X509Certificate2(certificateConfiguration.SigningCertificatePfxFilePath, certificateConfiguration.SigningCertificatePfxFilePassword));
						logger.LogInformation("AddSigningCredential ");
					}
					catch (CryptographicException e)
					{
						logger.LogError($"There was an error adding the key file - during the creation of the signing key {e.Message}");
					}
				}
				else
				{
					throw new Exception($"Signing key file: {certificateConfiguration.SigningCertificatePfxFilePath} not found");
				}
			}
			else if (certificateConfiguration.AzureKeyVault)
			{
				// Azure deployment, will be used if deployed to Azure				
				var keyVaultService = new KeyVaultCertificateService(certificateConfiguration.AzureKeyVaultEndPoint, certificateConfiguration.AzureKeyVaultClientId, certificateConfiguration.AzureKeyVaultClientSecret, logger);
				builder.AddSigningCredential(keyVaultService.GetCertificateFromKeyVault(certificateConfiguration.AzureKeyVaultCertificateName));
			}
			else if (certificateConfiguration.UseTemporarySigningKeyForDevelopment)
			{
				builder.AddDeveloperSigningCredential();
			}

			return builder;
		}

		/// <summary>
		/// Add custom validation key for signing key rollover
		/// http://docs.identityserver.io/en/latest/topics/crypto.html#signing-key-rollover
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configuration"></param>
		/// <param name="logger"></param>
		/// <returns></returns>
		public static IIdentityServerBuilder AddCustomValidationKey(this IIdentityServerBuilder builder, IConfiguration configuration, ILogger logger)
		{
			var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();

			if (certificateConfiguration.UseValidationCertificateThumbprint)
			{
				if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificateThumbprint))
				{
					throw new Exception(ValidationCertificateThumbprintNotFound);
				}

				var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				certStore.Open(OpenFlags.ReadOnly);

				var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.ValidationCertificateThumbprint, false);
				if (certCollection.Count == 0)
				{
					throw new Exception(CertificateNotFound);
				}

				var certificate = certCollection[0];

				builder.AddValidationKey(certificate);

			}
			else if (certificateConfiguration.UseValidationCertificatePfxFile)
			{
				if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificatePfxFilePath))
				{
					throw new Exception(ValidationCertificatePathIsNotSpecified);
				}

				if (File.Exists(certificateConfiguration.ValidationCertificatePfxFilePath))
				{
					try
					{
						builder.AddValidationKey(new X509Certificate2(certificateConfiguration.ValidationCertificatePfxFilePath, certificateConfiguration.ValidationCertificatePfxFilePassword));

					}
					catch (CryptographicException e)
					{
						logger.LogError($"There was an error adding the key file - during the creation of the validation key {e.Message}");
					}
				}
				else
				{
					throw new Exception($"Validation key file: {certificateConfiguration.SigningCertificatePfxFilePath} not found");
				}
			}
			return builder;
		}
	}
}
