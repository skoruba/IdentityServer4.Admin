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

        public ResourceMessage IdentityResourcePropertyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourcePropertyDoesNotExist),
                Description = IdentityResourceServiceResource.IdentityResourcePropertyDoesNotExist
            };
        }

        public ResourceMessage IdentityResourcePropertyExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourcePropertyExistsValue),
                Description = IdentityResourceServiceResource.IdentityResourcePropertyExistsValue
            };
        }

        public ResourceMessage IdentityResourcePropertyExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityResourcePropertyExistsKey),
                Description = IdentityResourceServiceResource.IdentityResourcePropertyExistsKey
            };
        }
    }
}
