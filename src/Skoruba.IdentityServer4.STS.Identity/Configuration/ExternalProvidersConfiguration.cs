namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class ExternalProvidersConfiguration
    {
        public bool UseGitHubProvider { get; set; }
        public string GitHubClientId { get; set; }
        public string GitHubClientSecret { get; set; }

        public bool UseAzureAdProvider { get; set; }
        public string AzureAdClientId { get; set; }
        public string AzureAdSecret { get; set; }
        public string AzureAdTenantId { get; set; }
        public string AzureInstance { get; set; }
        public string AzureAdCallbackPath { get; set; }
        public string AzureDomain { get; set; }
        
        public bool UseSaml2Provider { get; set; }
        public string Saml2OurEntityId { get; set; }
        public string Saml2TheirEntityId { get; set; }
        public string Saml2TheirMetadataLocation { get; set; }
        public string Saml2OurCertificatePath { get; set; }
        public string Saml2OurCertificatePassword { get; set; }
    }
}
