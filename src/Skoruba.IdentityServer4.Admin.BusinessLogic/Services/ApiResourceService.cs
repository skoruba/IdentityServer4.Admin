using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ApiResourceService<TDbContext> : IApiResourceService<TDbContext> 
        where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        private readonly IApiResourceRepository<TDbContext> _apiResourceRepository;
        private readonly IApiResourceServiceResources _apiResourceServiceResources;
        private readonly IClientService<TDbContext> _clientService;
        private const string SharedSecret = "SharedSecret";

        public ApiResourceService(IApiResourceRepository<TDbContext> apiResourceRepository,
            IApiResourceServiceResources apiResourceServiceResources,
            IClientService<TDbContext> clientService)
        {
            _apiResourceRepository = apiResourceRepository;
            _apiResourceServiceResources = apiResourceServiceResources;
            _clientService = clientService;
        }

        public async Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await _apiResourceRepository.GetApiResourcesAsync(search, page, pageSize);
            var apiResourcesDto = pagedList.ToModel();

            return apiResourcesDto;
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

        public ApiSecretsDto BuildApiSecretsViewModel(ApiSecretsDto apiSecrets)
        {
            apiSecrets.HashTypes = _clientService.GetHashTypes();
            apiSecrets.TypeList = _clientService.GetSecretTypes();

            return apiSecrets;
        }

        public async Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId)
        {
            var apiResource = await _apiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(_apiResourceServiceResources.ApiResourceDoesNotExist().Description, _apiResourceServiceResources.ApiResourceDoesNotExist().Description);
            var apiResourceDto = apiResource.ToModel();

            return apiResourceDto;
        }

        public async Task<int> AddApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(_apiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), _apiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
            }

            var resource = apiResource.ToEntity();

            return await _apiResourceRepository.AddApiResourceAsync(resource);
        }

        public async Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(_apiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), _apiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
            }

            var resource = apiResource.ToEntity();

            return await _apiResourceRepository.UpdateApiResourceAsync(resource);
        }

        public async Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource)
        {
            var resource = apiResource.ToEntity();

            return await _apiResourceRepository.DeleteApiResourceAsync(resource);
        }

        public async Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource)
        {
            var resource = apiResource.ToEntity();

            return await _apiResourceRepository.CanInsertApiResourceAsync(resource);
        }

        public async Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await _apiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(_apiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), _apiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await _apiResourceRepository.GetApiScopesAsync(apiResourceId, page, pageSize);

            var apiScopesDto = pagedList.ToModel();
            apiScopesDto.ApiResourceId = apiResourceId;
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public async Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            var apiResource = await _apiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(_apiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), _apiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var apiScope = await _apiResourceRepository.GetApiScopeAsync(apiResourceId, apiScopeId);
            if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(_apiResourceServiceResources.ApiScopeDoesNotExist().Description, apiScopeId), _apiResourceServiceResources.ApiScopeDoesNotExist().Description);

            var apiScopesDto = apiScope.ToModel();
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public async Task<int> AddApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModel(apiScope);
                throw new UserFriendlyViewException(string.Format(_apiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), _apiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
            }

            var scope = apiScope.ToEntity();

            return await _apiResourceRepository.AddApiScopeAsync(apiScope.ApiResourceId, scope);
        }

        public ApiScopesDto BuildApiScopeViewModel(ApiScopesDto apiScope)
        {
            ComboBoxHelpers.PopulateValuesToList(apiScope.UserClaimsItems, apiScope.UserClaims);

            return apiScope;
        }

        private async Task BuildApiScopesViewModel(ApiScopesDto apiScope)
        {
            if (apiScope.ApiScopeId == 0)
            {
                var apiScopesDto = await GetApiScopesAsync(apiScope.ApiResourceId);
                apiScope.Scopes.AddRange(apiScopesDto.Scopes);
                apiScope.TotalCount = apiScopesDto.TotalCount;
            }
        }

        public async Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModel(apiScope);
                throw new UserFriendlyViewException(string.Format(_apiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), _apiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
            }

            var scope = apiScope.ToEntity();

            return await _apiResourceRepository.UpdateApiScopeAsync(apiScope.ApiResourceId, scope);
        }

        public async Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope)
        {
            var scope = apiScope.ToEntity();

            return await _apiResourceRepository.DeleteApiScopeAsync(scope);
        }

        public async Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await _apiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(_apiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), _apiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await _apiResourceRepository.GetApiSecretsAsync(apiResourceId, page, pageSize);

            var apiSecretsDto = pagedList.ToModel();
            apiSecretsDto.ApiResourceId = apiResourceId;
            apiSecretsDto.ApiResourceName = await _apiResourceRepository.GetApiResourceNameAsync(apiResourceId);

            return apiSecretsDto;
        }

        public async Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret)
        {
            HashApiSharedSecret(apiSecret);

            var secret = apiSecret.ToEntity();
            
            return await _apiResourceRepository.AddApiSecretAsync(apiSecret.ApiResourceId, secret);
        }

        public async Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId)
        {
            var apiSecret = await _apiResourceRepository.GetApiSecretAsync(apiSecretId);
            if (apiSecret == null) throw new UserFriendlyErrorPageException(string.Format(_apiResourceServiceResources.ApiSecretDoesNotExist().Description, apiSecretId), _apiResourceServiceResources.ApiSecretDoesNotExist().Description);
            var apiSecretsDto = apiSecret.ToModel();

            return apiSecretsDto;
        }

        public async Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret)
        {
            var secret = apiSecret.ToEntity();

            return await _apiResourceRepository.DeleteApiSecretAsync(secret);
        }

        public async Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes)
        {
            var apiScope = apiScopes.ToEntity();

            return await _apiResourceRepository.CanInsertApiScopeAsync(apiScope);
        }

        public async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            return await _apiResourceRepository.GetApiResourceNameAsync(apiResourceId);
        }
    }
}
