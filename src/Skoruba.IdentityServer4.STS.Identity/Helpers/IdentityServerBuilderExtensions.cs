using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.STS.Identity.Configuration;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
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
        /// <returns></returns>
        public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder, IConfiguration configuration)
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
            else if (certificateConfiguration.UseSigningCertificateFile)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificateFilePath))
                {
                    throw new Exception(SigningCertificatePathIsNotSpecified);
                }

                if (File.Exists(certificateConfiguration.SigningCertificateFilePath))
                {
                    builder.AddSigningCredential(new X509Certificate2(certificateConfiguration.SigningCertificateFilePath, certificateConfiguration.SigningCertificateFilePassword));
                }
                else
                {
                    throw new Exception($"Signing key file: {certificateConfiguration.SigningCertificateFilePath} not found");
                }
            }
            else if (certificateConfiguration.UseTemporaryKey)
            {
                builder.AddDeveloperSigningCredential(persistKey: false);
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
            else if (certificateConfiguration.UseValidationCertificateFile)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificateFilePath))
                {
                    throw new Exception(ValidationCertificatePathIsNotSpecified);
                }

                if (File.Exists(certificateConfiguration.ValidationCertificateFilePath))
                {
                    builder.AddValidationKey(new X509Certificate2(certificateConfiguration.ValidationCertificateFilePath, certificateConfiguration.ValidationCertificateFilePassword));
                }
                else
                {
                    throw new Exception($"Validation key file: {certificateConfiguration.SigningCertificateFilePath} not found");
                }
            }

            return builder;
        }
    }
}
