using Skoruba.IdentityServer4.Admin.AspNetIdentity.Helpers;

namespace Skoruba.IdentityServer4.Admin.AspNetIdentity.Resources
{
    public interface IPersistedGrantAspNetIdentityServiceResources
    {
        ResourceMessage PersistedGrantDoesNotExist();

        ResourceMessage PersistedGrantWithSubjectIdDoesNotExist();
    }
}
