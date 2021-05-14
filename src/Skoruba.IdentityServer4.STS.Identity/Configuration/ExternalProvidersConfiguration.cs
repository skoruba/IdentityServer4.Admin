namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class ExternalProvidersConfiguration
    {
        public bool UseGitHubProvider { get; set; }
        public string GitHubClientId { get; set; }
        public string GitHubClientSecret { get; set; }        
        public string GitHubCallbackPath { get; set; }

        public bool UseAzureAdProvider { get; set; }
        public string AzureAdClientId { get; set; }
        public string AzureAdSecret { get; set; }
        public string AzureAdTenantId { get; set; }
        public string AzureInstance { get; set; }
        public string AzureAdCallbackPath { get; set; }
        public string AzureDomain { get; set; }

        public bool UseGoogleProvider { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
        public string GoogleCallbackPath { get; set; }
    }
}
