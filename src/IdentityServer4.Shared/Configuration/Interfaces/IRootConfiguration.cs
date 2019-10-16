using IdentityServer4.Shared.Configuration.Intefaces;

namespace IdentityServer4.Shared.Configuration.Interfaces
{
    public interface IRootConfiguration
    {
        IAdminAppConfiguration AdminAppConfiguration { get; }

        IRegisterConfiguration RegisterConfiguration { get; }
    }
}