using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Resources
{
    public class ClientServiceResources : IClientServiceResources
    {
        public virtual ResourceMessage ClientClaimDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientClaimDoesNotExist),
                Description = ClientServiceResource.ClientClaimDoesNotExist
            };
        }

        public virtual ResourceMessage ClientDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientDoesNotExist),
                Description = ClientServiceResource.ClientDoesNotExist
            };
        }

        public virtual ResourceMessage ClientExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientExistsKey),
                Description = ClientServiceResource.ClientExistsKey
            };
        }

        public virtual ResourceMessage ClientExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientExistsValue),
                Description = ClientServiceResource.ClientExistsValue
            };
        }

        public virtual ResourceMessage ClientPropertyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientPropertyDoesNotExist),
                Description = ClientServiceResource.ClientPropertyDoesNotExist
            };
        }

        public virtual ResourceMessage ClientSecretDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ClientSecretDoesNotExist),
                Description = ClientServiceResource.ClientSecretDoesNotExist
            };
        }
    }
}