namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class ExternalProvidersConfiguration
    {
        public bool UseFacebookProvider { get; set; }
        public string FacebookClientId { get; set; }
        public string FacebookClientSecret { get; set; }

        public bool UseGoogleProvider { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }

        public bool UseMicrosoftProvider { get; set; }
        public string MicrosoftClientId { get; set; }
        public string MicrosoftClientSecret { get; set; }

        public bool UseAppleProvider { get; set; }
        public string AppleClientId { get; set; }
        public string AppleClientSecret { get; set; }
    }
}
