namespace Skoruba.IdentityServer4.STS.Identity.Configuration.Interfaces
{
    public interface IAdminConfiguration
    {
        string IdentityAdminBaseUrl { get; }
        string AdministrationRole { get; }
    }
}