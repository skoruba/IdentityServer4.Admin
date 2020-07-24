namespace Skoruba.IdentityServer4.STS.KeyVault.Configuration
{
    public class AzureKeyVaultConfiguration
    {
        public string KeyVaultUri { get; set; }
        public string KeyName { get; set; }
        public string KeyIdentifier => $"{KeyVaultUri}/keys/{KeyName}";
    }
}
