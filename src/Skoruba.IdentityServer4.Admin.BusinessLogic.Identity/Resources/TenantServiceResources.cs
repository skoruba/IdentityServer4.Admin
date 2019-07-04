using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources
{
    public class TenantServiceResources : ITenantServiceResources
    {
        public ResourceMessage TenantDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(TenantDoesNotExist),
                Description = TenantServiceResource.TenantDoesNotExist
            };
        }
    }
}
