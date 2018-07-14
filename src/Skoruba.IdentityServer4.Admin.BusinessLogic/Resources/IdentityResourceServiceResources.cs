using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Resources
{
    public class IdentityResourceServiceResources : IIdentityResourceServiceResources
    {
        public virtual ResourceMessage IdentityResourceDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourceDoesNotExist),
                Description = IdentityResourceServiceResource.IdentityResourceDoesNotExist
            };
        }

        public virtual ResourceMessage IdentityResourceExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourceExistsKey),
                Description = IdentityResourceServiceResource.IdentityResourceExistsKey
            };
        }

        public virtual ResourceMessage IdentityResourceExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourceExistsValue),
                Description = IdentityResourceServiceResource.IdentityResourceExistsValue
            };
        }
    }
}
