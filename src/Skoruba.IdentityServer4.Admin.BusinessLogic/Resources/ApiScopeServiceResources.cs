using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Resources
{
    public class ApiScopeServiceResources : IApiScopeServiceResources
    {
        public virtual ResourceMessage ApiScopeDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopeDoesNotExist),
                Description = ApiScopeServiceResource.ApiScopeDoesNotExist
            };
        }

        public virtual ResourceMessage ApiScopeExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopeExistsValue),
                Description = ApiScopeServiceResource.ApiScopeExistsValue
            };
        }

        public virtual ResourceMessage ApiScopeExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopeExistsKey),
                Description = ApiScopeServiceResource.ApiScopeExistsKey
            };
        }

        public ResourceMessage ApiScopePropertyExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopePropertyExistsValue),
                Description = ApiScopeServiceResource.ApiScopePropertyExistsValue
            };
        }

        public ResourceMessage ApiScopePropertyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopePropertyDoesNotExist),
                Description = ApiScopeServiceResource.ApiScopePropertyDoesNotExist
            };
        }

        public ResourceMessage ApiScopePropertyExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiScopePropertyExistsKey),
                Description = ApiScopeServiceResource.ApiScopePropertyExistsKey
            };
        }
    }
}