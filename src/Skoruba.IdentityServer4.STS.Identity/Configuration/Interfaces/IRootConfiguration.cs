namespace Skoruba.IdentityServer4.STS.Identity.Configuration.Interfaces
{
    public interface IRootConfiguration
    {
        IAdminConfiguration AdminConfiguration { get; }

        IRegisterConfiguration RegisterConfiguration { get; }
    }
}