using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using SkorubaIdentityServer4Admin.Shared.Configuration.Common;
using SkorubaIdentityServer4Admin.Shared.Configuration.Email;
using SkorubaIdentityServer4Admin.Shared.Email;

namespace SkorubaIdentityServer4Admin.Shared.Helpers
{
    public static class StartupHelpers
    {
        /// <summary>
        /// Add email senders - configuration of sendgrid, smtp senders
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddEmailSenders(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpConfiguration = configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();
            var sendGridConfiguration = configuration.GetSection(nameof(SendGridConfiguration)).Get<SendGridConfiguration>();

            if (sendGridConfiguration != null && !string.IsNullOrWhiteSpace(sendGridConfiguration.ApiKey))
            {
                services.AddSingleton<ISendGridClient>(_ => new SendGridClient(sendGridConfiguration.ApiKey));
                services.AddSingleton(sendGridConfiguration);
                services.AddTransient<IEmailSender, SendGridEmailSender>();
            }
            else if (smtpConfiguration != null && !string.IsNullOrWhiteSpace(smtpConfiguration.Host))
            {
                services.AddSingleton(smtpConfiguration);
                services.AddTransient<IEmailSender, SmtpEmailSender>();
            }
            else
            {
                services.AddSingleton<IEmailSender, LogEmailSender>();
            }
        }

        public static void AddDataProtection<TDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TDbContext : DbContext, IDataProtectionKeyContext
        {
            var dataProtectionConfiguration = configuration.GetSection(nameof(DataProtectionConfiguration)).Get<DataProtectionConfiguration>();
            var azureKeyVaultConfiguration = configuration.GetSection(nameof(AzureKeyVaultConfiguration)).Get<AzureKeyVaultConfiguration>();

            var dataProtectionBuilder = services.AddDataProtection()
                .SetApplicationName("SkorubaIdentityServer4Admin")
                .PersistKeysToDbContext<TDbContext>();

            if (dataProtectionConfiguration.ProtectKeysWithAzureKeyVault)
            {
                if (azureKeyVaultConfiguration.UseClientCredentials)
                {
                    dataProtectionBuilder.ProtectKeysWithAzureKeyVault(azureKeyVaultConfiguration.DataProtectionKeyIdentifier, azureKeyVaultConfiguration.ClientId, azureKeyVaultConfiguration.ClientSecret);
                }
                else
                {
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                    dataProtectionBuilder.ProtectKeysWithAzureKeyVault(keyVaultClient, azureKeyVaultConfiguration.DataProtectionKeyIdentifier);
                }
            }
        }

        public static void AddAzureKeyVaultConfiguration(this IConfiguration configuration, IConfigurationBuilder configurationBuilder)
        {
            if (configuration.GetSection(nameof(AzureKeyVaultConfiguration)).Exists())
            {
                var azureKeyVaultConfiguration = configuration.GetSection(nameof(AzureKeyVaultConfiguration)).Get<AzureKeyVaultConfiguration>();

                if (azureKeyVaultConfiguration.ReadConfigurationFromKeyVault)
                {
                    if (azureKeyVaultConfiguration.UseClientCredentials)
                    {
                        configurationBuilder.AddAzureKeyVault(azureKeyVaultConfiguration.AzureKeyVaultEndpoint,
                            azureKeyVaultConfiguration.ClientId, azureKeyVaultConfiguration.ClientSecret);
                    }
                    else
                    {
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider()
                                .KeyVaultTokenCallback));

                        configurationBuilder.AddAzureKeyVault(azureKeyVaultConfiguration.AzureKeyVaultEndpoint,
                            keyVaultClient, new DefaultKeyVaultSecretManager());
                    }
                }
            }
        }
    }
}






