using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.STS.KeyVault.Configuration;
using Skoruba.IdentityServer4.STS.KeyVault.Stores;
using Skoruba.IdentityServer4.STS.KeyVault.Tokens;

namespace Skoruba.IdentityServer4.STS.KeyVault
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKeyVaultSigningKeyFeature(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var authenticationCallback = new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
            var keyVaultClient = new KeyVaultClient(authenticationCallback);
            var azureKeyVaultConfiguration = new AzureKeyVaultConfiguration();
            configuration.GetSection("AzureKeyVaultConfiguration").Bind(azureKeyVaultConfiguration);
            
            services.AddSingleton<IKeyVaultClient>(keyVaultClient)
                .AddSingleton(azureKeyVaultConfiguration)
                .AddTransient<ITokenCreationService, KeyVaultTokenCreationService>()
                .AddTransient<ISigningCredentialStore, AzureKeyVaultKeyStore>()
                .AddTransient<IValidationKeysStore, AzureKeyVaultKeyStore>();

            return services;
        }
    }
}
