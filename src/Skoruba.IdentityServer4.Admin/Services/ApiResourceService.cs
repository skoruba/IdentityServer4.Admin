using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Localization;
using Skoruba.IdentityServer4.Admin.Data.Mappers;
using Skoruba.IdentityServer4.Admin.Data.Repositories;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Helpers;
using Skoruba.IdentityServer4.Admin.ViewModels.Common;
using Skoruba.IdentityServer4.Admin.ViewModels.Configuration;

namespace Skoruba.IdentityServer4.Admin.Services
{
    public class ApiResourceService : IApiResourceService
    {
        private readonly IApiResourceRepository _apiResourceRepository;
        private readonly IStringLocalizer<ApiResourceService> _localizer;
        private readonly IClientService _clientService;
        private const string SharedSecret = "SharedSecret";

        public ApiResourceService(IApiResourceRepository apiResourceRepository,
            IStringLocalizer<ApiResourceService> localizer,
            IClientService clientService)
        {
            _apiResourceRepository = apiResourceRepository;
            _localizer = localizer;
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
            apiSecrets.HashTypes = ComboBoxHelpers.GetData(_clientService.GetHashTypes());
            apiSecrets.TypeList = ComboBoxHelpers.GetData(_clientService.GetSecretTypes());

            return apiSecrets;
        }

        public async Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId)
        {
            var apiResource = await _apiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(_localizer["ApiResourceDoesNotExist"], _localizer["ApiResourceDoesNotExist"]);
            var apiResourceDto = apiResource.ToModel();

            return apiResourceDto;
        }

        public async Task<int> AddApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(_localizer["ApiResourceExistsValue"], apiResource.Name), _localizer["ApiResourceExistsKey"], apiResource);
            }

            var resource = apiResource.ToEntity();

            return await _apiResourceRepository.AddApiResourceAsync(resource);
        }

        public async Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(_localizer["ApiResourceExistsValue"], apiResource.Name), _localizer["ApiResourceExistsKey"], apiResource);
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
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(_localizer["ApiResourceDoesNotExist"], apiResourceId), _localizer["ApiResourceDoesNotExist"]);

            var pagedList = await _apiResourceRepository.GetApiScopesAsync(apiResourceId, page, pageSize);

            var apiScopesDto = pagedList.ToModel();
            apiScopesDto.ApiResourceId = apiResourceId;
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public async Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            var apiResource = await _apiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(_localizer["ApiResourceDoesNotExist"], apiResourceId), _localizer["ApiResourceDoesNotExist"]);

            var apiScope = await _apiResourceRepository.GetApiScopeAsync(apiResourceId, apiScopeId);
            if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(_localizer["ApiScopeDoesNotExist"], apiScopeId), _localizer["ApiScopeDoesNotExist"]);

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
                throw new UserFriendlyViewException(string.Format(_localizer["ApiScopeExistsValue"], apiScope.Name), _localizer["ApiScopeExistsKey"], apiScope);
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
                throw new UserFriendlyViewException(string.Format(_localizer["ApiScopeExistsValue"], apiScope.Name), _localizer["ApiScopeExistsKey"], apiScope);
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
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(_localizer["ApiResourceDoesNotExist"], apiResourceId), _localizer["ApiResourceDoesNotExist"]);

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
            if (apiSecret == null) throw new UserFriendlyErrorPageException(string.Format(_localizer["ApiSecretDoesNotExist"], apiSecretId), _localizer["ApiSecretDoesNotExist"]);
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
