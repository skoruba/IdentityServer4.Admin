using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using SkorubaIdentityServer4Admin.STS.Identity.Configuration;

namespace SkorubaIdentityServer4Admin.STS.Identity.Helpers
{
	public static class IdentityServerBuilderExtensions
	{
		private const string CertificateNotFound = "Certificate not found";
		private const string CertificateThumbprintNotFound = "Signing certificate thumbprint not found";

		/// <summary>
		/// Add custom signing certificate from certification store according thumbprint
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder, IConfiguration configuration)
		{
			var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();

			if (certificateConfiguration.UseTemporaryKey)
			{
				builder.AddDeveloperSigningCredential(persistKey: false);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificateThumbprint))
				{					
					throw new Exception(CertificateThumbprintNotFound);
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

			return builder;
		}

		/// <summary>
		/// Add custom validation key for signing key rollover
		/// http://docs.identityserver.io/en/latest/topics/crypto.html#signing-key-rollover
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static IIdentityServerBuilder AddCustomValidationKey(this IIdentityServerBuilder builder, IConfiguration configuration)
		{
			var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();

			if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificateThumbprint)) return builder;

			var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			certStore.Open(OpenFlags.ReadOnly);

			var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.ValidationCertificateThumbprint, false);
			if (certCollection.Count == 0)
			{
				throw new Exception(CertificateNotFound);
			}

			var certificate = certCollection[0];

			builder.AddValidationKey(certificate);

			return builder;
		}
	}
}
