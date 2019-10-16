namespace IdentityServer4.Shared.Configuration.Interfaces
{
    public interface IAdminAppConfiguration
    {
        string IdentityAdminRedirectUri { get; }
        string IdentityServerBaseUrl { get; }
        string IdentityAdminBaseUrl { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string OidcResponseType { get; }
        string[] Scopes { get; }
        string IdentityAdminApiSwaggerUIClientId { get; }
        string IdentityAdminApiSwaggerUIRedirectUrl { get; }
        string IdentityAdminApiScope { get; }
    }
}