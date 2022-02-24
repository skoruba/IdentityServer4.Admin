namespace Skoruba.IdentityServer4.Shared.Configuration.Configuration.Identity
{
    public class RegisterConfiguration
    {
        public bool Enabled { get; set; } = true;
        
        public string ApiConfirmationUrl { get; set; }

        public string NewUserDefaultRole { get; set; }

		public string EmailTemplatesBaseDirectory { get; set; }
    }
}
