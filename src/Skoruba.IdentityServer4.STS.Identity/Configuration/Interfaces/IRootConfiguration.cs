using Skoruba.IdentityServer4.Shared.Configuration.Identity;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration.Interfaces
{
    public interface IRootConfiguration
    {
        AdminConfiguration AdminConfiguration { get; }

        RegisterConfiguration RegisterConfiguration { get; }
    }
}