namespace Skoruba.IdentityServer4.STS.Identity.Configuration.Intefaces
{
    public interface IRootConfiguration
    {
        IAdminAppConfiguration AdminAppConfiguration { get; }

        IRegisterConfiguration RegisterConfiguration { get; }
    }
}