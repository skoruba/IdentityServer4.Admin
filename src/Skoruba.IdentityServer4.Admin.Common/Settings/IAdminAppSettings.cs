namespace Skoruba.IdentityServer4.Admin.Common.Settings
{
    public interface IAdminAppSettings
    {
        string AdministrationRole { get; }
        string IdentityAdminBaseUrl { get; }
        string IdentityAdminCookieName { get; }
        string IdentityAdminRedirectUri { get; }
        string IdentityServerBaseUrl { get; }
        string OidcClientId { get; }
    }
}