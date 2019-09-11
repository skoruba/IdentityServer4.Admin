namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class LoginConfiguration
    {
        public LoginResolutionPolicy ResolutionPolicy { get; set; } = LoginResolutionPolicy.Username;
    }
}
