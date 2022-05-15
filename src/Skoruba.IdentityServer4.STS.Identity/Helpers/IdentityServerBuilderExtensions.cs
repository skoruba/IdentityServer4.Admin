using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertificateManager;
using CertificateManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Common;
using Skoruba.IdentityServer4.Shared.Configuration.Helpers;

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
        /// <param name="webHostEnvironment"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();
            var azureKeyVaultConfiguration = configuration.GetSection(nameof(AzureKeyVaultConfiguration)).Get<AzureKeyVaultConfiguration>();

            if (certificateConfiguration.UseSigningCertificateThumbprint)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificateThumbprint))
                {
                    throw new Exception(SigningCertificateThumbprintNotFound);
                }

                StoreLocation storeLocation = StoreLocation.LocalMachine;
                bool validOnly = certificateConfiguration.CertificateValidOnly;

                // Parse the Certificate StoreLocation
                string certStoreLocationLower = certificateConfiguration.CertificateStoreLocation.ToLower();
                if (certStoreLocationLower == StoreLocation.CurrentUser.ToString().ToLower() ||
                    certificateConfiguration.CertificateStoreLocation == ((int)StoreLocation.CurrentUser).ToString())
                {
                    storeLocation = StoreLocation.CurrentUser;
                }
                else if (certStoreLocationLower == StoreLocation.LocalMachine.ToString().ToLower() ||
                         certStoreLocationLower == ((int)StoreLocation.LocalMachine).ToString())
                {
                    storeLocation = StoreLocation.LocalMachine;
                }
                else { storeLocation = StoreLocation.LocalMachine; validOnly = true; }

                // Open Certificate
                var certStore = new X509Store(StoreName.My, storeLocation);
                certStore.Open(OpenFlags.ReadOnly);

                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.SigningCertificateThumbprint, validOnly);
                if (certCollection.Count == 0)
                {
                    throw new Exception(CertificateNotFound);
                }

                var certificate = certCollection[0];

                builder.AddSigningCredential(certificate);
            }
            else if (certificateConfiguration.UseSigningCertificateForAzureKeyVault)
            {
                var x509Certificate2Certs = AzureKeyVaultHelpers.GetCertificates(azureKeyVaultConfiguration).GetAwaiter().GetResult();

                builder.AddSigningCredential(x509Certificate2Certs.ActiveCertificate);
            }
            else if (certificateConfiguration.UseSigningCertificatePfxFile)
            {
                if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificatePfxFilePath))
                {
                    throw new Exception(SigningCertificatePathIsNotSpecified);
                }

                string certificateFilePath = Path.Combine(webHostEnvironment.ContentRootPath, certificateConfiguration.SigningCertificatePfxFilePath);
                if (!File.Exists(certificateFilePath) || certificateConfiguration.NeedToCreateNewOne)
                {
                    CreateRsaCertificate(builder.Services, certificateConfiguration, certificateFilePath);
                }

                if (File.Exists(certificateFilePath))
                {
                    try
                    {
                        builder.AddSigningCredential(new X509Certificate2(certificateFilePath, certificateConfiguration.SigningCertificatePfxFilePassword));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("There was an error adding the key file - during the creation of the signing key", e);
                    }
                }
                else
                {
                    throw new Exception($"Signing key file: {certificateConfiguration.SigningCertificatePfxFilePath} not found");
                }
            }
            else if (certificateConfiguration.UseTemporarySigningKeyForDevelopment)
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("Signing credential is not specified");
            }

            return builder;
        }

        /// <summary>
        /// Create RSA Certificate For Signing Credential - https://damienbod.com/2020/02/10/create-certificates-for-identityserver4-signing-using-net-core/
        /// </summary>
        /// <param name="services"></param>
        /// <param name="certificateConfiguration"></param>
        /// <param name="certificateFilePath"></param>
        private static void CreateRsaCertificate(IServiceCollection services, CertificateConfiguration certificateConfiguration, string certificateFilePath)
        {
            services.AddCertificateManager();
            var createCertificates = services.BuildServiceProvider().GetRequiredService<CreateCertificates>();

            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = false,
                HasPathLengthConstraint = false,
                PathLengthConstraint = 0,
                Critical = false
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    certificateConfiguration.DnsName
                }
            };

            const X509KeyUsageFlags x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature;

            // only if certification authentication is used
            var enhancedKeyUsages = new OidCollection
            {
                OidLookup.ClientAuthentication,
                OidLookup.ServerAuthentication
            };

            var certificate = createCertificates.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = certificateConfiguration.DnsName },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow,
                    ValidTo = DateTimeOffset.UtcNow.AddYears(certificateConfiguration.ValidityPeriodInYear)
                },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration
                {
                    KeySize = 2048,
                    HashAlgorithmName = HashAlgorithmName.SHA512
                });

            var iec = services.BuildServiceProvider().GetRequiredService<ImportExportCertificate>();
            var rsaCertPfxBytes = iec.ExportSelfSignedCertificatePfx(certificateConfiguration.SigningCertificatePfxFilePassword, certificate);
            File.WriteAllBytes(certificateFilePath, rsaCertPfxBytes);
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
            var azureKeyVaultConfiguration = configuration.GetSection(nameof(AzureKeyVaultConfiguration)).Get<AzureKeyVaultConfiguration>();

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
            else if (certificateConfiguration.UseValidationCertificateForAzureKeyVault)
            {
                var x509Certificate2Certs = AzureKeyVaultHelpers.GetCertificates(azureKeyVaultConfiguration).GetAwaiter().GetResult();

                if (x509Certificate2Certs.SecondaryCertificate != null)
                {
                    builder.AddValidationKey(x509Certificate2Certs.SecondaryCertificate);
                }
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
                    catch (Exception e)
                    {
                        throw new Exception("There was an error adding the key file - during the creation of the validation key", e);
                    }
                }
                else
                {
                    throw new Exception($"Validation key file: {certificateConfiguration.ValidationCertificatePfxFilePath} not found");
                }
            }

            return builder;
        }
    }
}
