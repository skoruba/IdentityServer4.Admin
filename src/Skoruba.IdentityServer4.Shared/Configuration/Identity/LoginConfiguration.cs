namespace Skoruba.IdentityServer4.Shared.Configuration.Identity
{
    public class LoginConfiguration
    {
        public LoginResolutionPolicy ResolutionPolicy { get; set; } = LoginResolutionPolicy.Username;

        public bool RequireUniqueEmail { get; set; } = true;
    }
}
