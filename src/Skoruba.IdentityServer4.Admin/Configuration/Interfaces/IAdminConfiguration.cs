namespace Skoruba.IdentityServer4.Admin.Configuration.Interfaces
{
    public interface IAdminConfiguration
    {
        string IdentityAdminRedirectUri { get; }

        string IdentityServerBaseUrl { get; }

        string IdentityAdminBaseUrl { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string OidcResponseType { get; }
        string[] Scopes { get; }
    }
}