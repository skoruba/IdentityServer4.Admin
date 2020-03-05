namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class ExternalProvidersConfiguration
    {
        public bool UseGitHubProvider { get; set; }
        public string GitHubClientId { get; set; }
        public string GitHubClientSecret { get; set; }
		public bool UseSaml2Provider { get; set; }
		public string Saml2OurEntityId { get; set; }
        public string Saml2TheirEntityId { get; set; }
        public string Saml2TheirMetadataLocation { get; set; }
        public string Saml2OurCertificatePath { get; set; }
        public string Saml2OurCertificatePassword { get; set; }
    }
}
