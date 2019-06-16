using System.Threading.Tasks;
using IdentityServer4.Models;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Helpers;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ApiResourceService : IApiResourceService
    {
        protected readonly IApiResourceRepository ApiResourceRepository;
        protected readonly IApiResourceServiceResources ApiResourceServiceResources;
        protected readonly IClientService ClientService;
        private const string SharedSecret = "SharedSecret";

        public ApiResourceService(IApiResourceRepository apiResourceRepository,
            IApiResourceServiceResources apiResourceServiceResources,
            IClientService clientService)
        {
            ApiResourceRepository = apiResourceRepository;
            ApiResourceServiceResources = apiResourceServiceResources;
            ClientService = clientService;
        }

        public virtual async Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await ApiResourceRepository.GetApiResourcesAsync(search, page, pageSize);
            var apiResourcesDto = pagedList.ToModel();

            return apiResourcesDto;
        }

        public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await ApiResourceRepository.GetApiResourcePropertiesAsync(apiResourceId, page, pageSize);
            var apiResourcePropertiesDto = pagedList.ToModel();
            apiResourcePropertiesDto.ApiResourceId = apiResourceId;
            apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);

            return apiResourcePropertiesDto;
        }

        public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId)
        {
            var apiResourceProperty = await ApiResourceRepository.GetApiResourcePropertyAsync(apiResourcePropertyId);
            if (apiResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourcePropertyDoesNotExist().Description, apiResourcePropertyId));

            var apiResourcePropertiesDto = apiResourceProperty.ToModel();
            apiResourcePropertiesDto.ApiResourceId = apiResourceProperty.ApiResourceId;
            apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceProperty.ApiResourceId);

            return apiResourcePropertiesDto;
        }

        public virtual async Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties)
        {
            var canInsert = await CanInsertApiResourcePropertyAsync(apiResourceProperties);
            if (!canInsert)
            {
                await BuildApiResourcePropertiesViewModelAsync(apiResourceProperties);
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourcePropertyExistsValue().Description, apiResourceProperties.Key), ApiResourceServiceResources.ApiResourcePropertyExistsKey().Description, apiResourceProperties);
            }

            var apiResourceProperty = apiResourceProperties.ToEntity();

            return await ApiResourceRepository.AddApiResourcePropertyAsync(apiResourceProperties.ApiResourceId, apiResourceProperty);
        }

        public virtual async Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
        {
            var propertyEntity = apiResourceProperty.ToEntity();

            return await ApiResourceRepository.DeleteApiResourcePropertyAsync(propertyEntity);
        }

        public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
        {
            var resource = apiResourceProperty.ToEntity();

            return await ApiResourceRepository.CanInsertApiResourcePropertyAsync(resource);
        }

        private void HashApiSharedSecret(ApiSecretsDto apiSecret)
        {
            if (apiSecret.Type != SharedSecret) return;

            if (apiSecret.HashType == ((int)HashType.Sha256).ToString())
            {
                apiSecret.Value = apiSecret.Value.Sha256();
            }
            else if (apiSecret.HashType == ((int)HashType.Sha512).ToString())
            {
                apiSecret.Value = apiSecret.Value.Sha512();
            }
        }

        public virtual ApiSecretsDto BuildApiSecretsViewModel(ApiSecretsDto apiSecrets)
        {
            apiSecrets.HashTypes = ClientService.GetHashTypes();
            apiSecrets.TypeList = ClientService.GetSecretTypes();

            return apiSecrets;
        }

        public virtual async Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
            var apiResourceDto = apiResource.ToModel();

            return apiResourceDto;
        }

        public virtual async Task<int> AddApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
            }

            var resource = apiResource.ToEntity();

            return await ApiResourceRepository.AddApiResourceAsync(resource);
        }

        public virtual async Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
            }

            var resource = apiResource.ToEntity();

            return await ApiResourceRepository.UpdateApiResourceAsync(resource);
        }

        public virtual async Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource)
        {
            var resource = apiResource.ToEntity();

            return await ApiResourceRepository.DeleteApiResourceAsync(resource);
        }

        public virtual async Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource)
        {
            var resource = apiResource.ToEntity();

            return await ApiResourceRepository.CanInsertApiResourceAsync(resource);
        }

        public virtual async Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await ApiResourceRepository.GetApiScopesAsync(apiResourceId, page, pageSize);

            var apiScopesDto = pagedList.ToModel();
            apiScopesDto.ApiResourceId = apiResourceId;
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public virtual async Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var apiScope = await ApiResourceRepository.GetApiScopeAsync(apiResourceId, apiScopeId);
            if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiScopeDoesNotExist().Description, apiScopeId), ApiResourceServiceResources.ApiScopeDoesNotExist().Description);

            var apiScopesDto = apiScope.ToModel();
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public virtual async Task<int> AddApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModelAsync(apiScope);
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
            }

            var scope = apiScope.ToEntity();

            return await ApiResourceRepository.AddApiScopeAsync(apiScope.ApiResourceId, scope);
        }

        public virtual ApiScopesDto BuildApiScopeViewModel(ApiScopesDto apiScope)
        {
            ComboBoxHelpers.PopulateValuesToList(apiScope.UserClaimsItems, apiScope.UserClaims);

            return apiScope;
        }

        private async Task BuildApiScopesViewModelAsync(ApiScopesDto apiScope)
        {
            if (apiScope.ApiScopeId == 0)
            {
                var apiScopesDto = await GetApiScopesAsync(apiScope.ApiResourceId);
                apiScope.Scopes.AddRange(apiScopesDto.Scopes);
                apiScope.TotalCount = apiScopesDto.TotalCount;
            }
        }

        private async Task BuildApiResourcePropertiesViewModelAsync(ApiResourcePropertiesDto apiResourceProperties)
        {
            var apiResourcePropertiesDto = await GetApiResourcePropertiesAsync(apiResourceProperties.ApiResourceId);
            apiResourceProperties.ApiResourceProperties.AddRange(apiResourcePropertiesDto.ApiResourceProperties);
            apiResourceProperties.TotalCount = apiResourcePropertiesDto.TotalCount;
        }

        public virtual async Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModelAsync(apiScope);
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
            }

            var scope = apiScope.ToEntity();

            return await ApiResourceRepository.UpdateApiScopeAsync(apiScope.ApiResourceId, scope);
        }

        public virtual async Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope)
        {
            var scope = apiScope.ToEntity();

            return await ApiResourceRepository.DeleteApiScopeAsync(scope);
        }

        public virtual async Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await ApiResourceRepository.GetApiSecretsAsync(apiResourceId, page, pageSize);

            var apiSecretsDto = pagedList.ToModel();
            apiSecretsDto.ApiResourceId = apiResourceId;
            apiSecretsDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);

            return apiSecretsDto;
        }

        public virtual async Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret)
        {
            HashApiSharedSecret(apiSecret);

            var secret = apiSecret.ToEntity();

            return await ApiResourceRepository.AddApiSecretAsync(apiSecret.ApiResourceId, secret);
        }

        public virtual async Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId)
        {
            var apiSecret = await ApiResourceRepository.GetApiSecretAsync(apiSecretId);
            if (apiSecret == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiSecretDoesNotExist().Description, apiSecretId), ApiResourceServiceResources.ApiSecretDoesNotExist().Description);
            var apiSecretsDto = apiSecret.ToModel();

            return apiSecretsDto;
        }

        public virtual async Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret)
        {
            var secret = apiSecret.ToEntity();

            return await ApiResourceRepository.DeleteApiSecretAsync(secret);
        }

        public virtual async Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes)
        {
            var apiScope = apiScopes.ToEntity();

            return await ApiResourceRepository.CanInsertApiScopeAsync(apiScope);
        }

        public virtual async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            return await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);
        }
    }
}
