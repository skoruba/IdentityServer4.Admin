namespace Skoruba.IdentityServer4.Shared.Configuration.Identity
{
    // From where should the login be sourced
    // by default it's sourced from Username
    public enum LoginResolutionPolicy
    {
        Username = 0,
        Email = 1
    }
}
