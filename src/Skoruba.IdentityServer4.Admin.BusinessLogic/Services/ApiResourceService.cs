using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource;
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
        protected readonly IAuditEventLogger AuditEventLogger;
        private const string SharedSecret = "SharedSecret";

        public ApiResourceService(IApiResourceRepository apiResourceRepository,
            IApiResourceServiceResources apiResourceServiceResources,
            IClientService clientService,
            IAuditEventLogger auditEventLogger)
        {
            ApiResourceRepository = apiResourceRepository;
            ApiResourceServiceResources = apiResourceServiceResources;
            ClientService = clientService;
            AuditEventLogger = auditEventLogger;
        }

		#region ApiResource

		public virtual async Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId)
		{
			var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
			if (apiResource == null) throw new UserFriendlyErrorPageException(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
			var apiResourceDto = apiResource.ToModel();

			await AuditEventLogger.LogEventAsync(new ApiResourceRequestedEvent(apiResourceId, apiResourceDto));

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

			var added = await ApiResourceRepository.AddApiResourceAsync(resource);

			await AuditEventLogger.LogEventAsync(new ApiResourceAddedEvent(apiResource));

			return added;
		}

		public virtual async Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource)
		{
			var canInsert = await CanInsertApiResourceAsync(apiResource);
			if (!canInsert)
			{
				throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
			}

			var resource = apiResource.ToEntity();

			var originalApiResource = await GetApiResourceAsync(apiResource.Id);

			var updated = await ApiResourceRepository.UpdateApiResourceAsync(resource);

			await AuditEventLogger.LogEventAsync(new ApiResourceUpdatedEvent(originalApiResource, apiResource));

			return updated;
		}

		public virtual async Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource)
		{
			var resource = apiResource.ToEntity();

			var deleted = await ApiResourceRepository.DeleteApiResourceAsync(resource);

			await AuditEventLogger.LogEventAsync(new ApiResourceDeletedEvent(apiResource));

			return deleted;
		}

		public virtual async Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource)
		{
			var resource = apiResource.ToEntity();

			return await ApiResourceRepository.CanInsertApiResourceAsync(resource);
		}

		#endregion

		#region ApiResourceProperties

		private async Task BuildApiResourcePropertiesViewModelAsync(ApiResourcePropertiesDto apiResourceProperties)
		{
			var apiResourcePropertiesDto = await GetApiResourcePropertiesAsync(apiResourceProperties.ApiResourceId);
			apiResourceProperties.ApiResourceProperties.AddRange(apiResourcePropertiesDto.ApiResourceProperties);
			apiResourceProperties.TotalCount = apiResourcePropertiesDto.TotalCount;
		}

		public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
		{
			var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
			if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

			var pagedList = await ApiResourceRepository.GetApiResourcePropertiesAsync(apiResourceId, page, pageSize);
			var apiResourcePropertiesDto = pagedList.ToModel();
			apiResourcePropertiesDto.ApiResourceId = apiResourceId;
			apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceSecretNameAsync(apiResourceId);

			return apiResourcePropertiesDto;
		}


		public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId)
		{
			var apiResourceProperty = await ApiResourceRepository.GetApiResourcePropertyAsync(apiResourcePropertyId);
			if (apiResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourcePropertyDoesNotExist().Description, apiResourcePropertyId));

			var apiResourcePropertiesDto = apiResourceProperty.ToModel();
			apiResourcePropertiesDto.ApiResourceId = apiResourceProperty.ApiResourceId;
			apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceSecretNameAsync(apiResourceProperty.ApiResourceId);

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

			var saved = await ApiResourceRepository.AddApiResourcePropertyAsync(apiResourceProperties.ApiResourceId, apiResourceProperty);

			return saved;
		}

		public virtual async Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
		{
			var propertyEntity = apiResourceProperty.ToEntity();

			var deleted = await ApiResourceRepository.DeleteApiResourcePropertyAsync(propertyEntity);

			return deleted;
		}

		public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
		{
			var resource = apiResourceProperty.ToEntity();

			return await ApiResourceRepository.CanInsertApiResourcePropertyAsync(resource);
		}

		private async Task<bool> CanInsertApiResourceScopeAsync(ApiResourceScopeDto apiResourceScopeDto)
		{
			var scope = apiResourceScopeDto.ToEntity();

			return await ApiResourceRepository.CanInsertApiResourceScopeAsync(scope);
		}

		#endregion

		#region ApiScope

		private async Task BuildApiScopesViewModelAsync(ApiScopesDto apiScope)
		{
			if (apiScope.ApiResourceId == 0)
			{
				var apiScopesDto = await GetApiScopesAsync(apiScope.ApiResourceId);
				apiScope.Scopes.AddRange(apiScopesDto.Scopes);
				apiScope.TotalCount = apiScopesDto.TotalCount;
			}
		}

		public async virtual Task<ApiScopesDto> BuildApiScopeViewModelAsync(ApiScopesDto apiScope)
		{
			ComboBoxHelpers.PopulateValuesToList(apiScope.UserClaimsItems, apiScope.UserClaims);
			apiScope.ResourceName = await GetApiResourceNameAsync(apiScope.ApiResourceId);

			return apiScope;
		}

		public virtual async Task<ApiScopesDto> GetApiScopeAsync(int apiScopeId)
		{
			var apiScope = await ApiResourceRepository.GetApiScopeAsync(apiScopeId);
			if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiScopeDoesNotExist().Description, apiScopeId), ApiResourceServiceResources.ApiScopeDoesNotExist().Description);

			var apiScopesDto = apiScope.ToModel();

			await AuditEventLogger.LogEventAsync(new ApiScopeRequestedEvent(apiScopesDto));

			return apiScopesDto;
		}

		public virtual async Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
		{
			var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
			if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

			var apiScope = await ApiResourceRepository.GetApiScopesAsync(apiResourceId);
			if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiScopeDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiScopeDoesNotExist().Description);

			var apiScopesDto = apiScope.ToModel();
			apiScopesDto.ApiResourceId = apiResourceId;
			apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

			await AuditEventLogger.LogEventAsync(new ApiScopesRequestedEvent(apiScopesDto));

			return apiScopesDto;
		}

		public virtual async Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId)
		{
			var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var apiScope = await ApiResourceRepository.GetApiScopeAsync(apiScopeId);
			if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiScopeDoesNotExist().Description, apiScopeId), ApiResourceServiceResources.ApiScopeDoesNotExist().Description);

			var apiScopesDto = apiScope.ToModel();
			apiScopesDto.ApiResourceId = apiResourceId;
			apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

			await AuditEventLogger.LogEventAsync(new ApiScopesRequestedEvent(apiScopesDto));

			return apiScopesDto;
		}


		public virtual async Task<int> AddApiScopeAsync(int apiResourceId, ApiScopesDto apiScope)
		{
			var canInsert = await CanInsertApiScopeAsync(apiScope);
			if (!canInsert)
			{
				await BuildApiScopesViewModelAsync(apiScope);
				throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
			}

			var scope = apiScope.ToEntity();

			var added = await ApiResourceRepository.AddApiScopeAsync(apiResourceId, scope);

			await AuditEventLogger.LogEventAsync(new ApiScopeAddedEvent(apiScope));

			return added;
		}

		public virtual async Task<int> UpdateApiScopeAsync(ApiScopesDto apiScopesDto)
		{
			var canInsert = await CanInsertApiScopeAsync(apiScopesDto);
			if (!canInsert)
			{
				await BuildApiScopesViewModelAsync(apiScopesDto);
				throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiScopeExistsValue().Description, apiScopesDto.Name), ApiResourceServiceResources.ApiScopeExistsKey().Description, apiScopesDto);
			}

			var scope = apiScopesDto.ToEntity();

			var originalApiScope = await GetApiScopeAsync(apiScopesDto.ApiScopeId);

			var updated = await ApiResourceRepository.UpdateApiScopeAsync(apiScopesDto.ApiResourceId, scope);

			await AuditEventLogger.LogEventAsync(new ApiScopeUpdatedEvent(originalApiScope, apiScopesDto));

			return updated;
		}

        public virtual async Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope)
		{
			var scope = apiScope.ToEntity();

			var deleted = await ApiResourceRepository.DeleteApiScopeAsync(scope);

			await AuditEventLogger.LogEventAsync(new ApiScopeDeletedEvent(apiScope));

			return deleted;
		}


		public virtual async Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes)
		{
			var apiScope = apiScopes.ToEntity();

			return await ApiResourceRepository.CanInsertApiResourceSecretAsync(apiScope);
		}

		#endregion

		#region ApiSecrets

		public virtual ApiResourceSecretsDto BuildApiSecretsViewModel(ApiResourceSecretsDto apiSecrets)
		{

			apiSecrets.HashTypes = ClientService.GetHashTypes();
			apiSecrets.TypeList = ClientService.GetSecretTypes();

			return apiSecrets;
		}

		public virtual async Task<ApiResourceSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
		{
			var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
			if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

			var pagedList = await ApiResourceRepository.GetApiResourceSecretsAsync(apiResourceId, page, pageSize);

			var apiSecretsDto = pagedList.ToModel();
			apiSecretsDto.ApiResourceId = apiResourceId;
			apiSecretsDto.ApiResourceName = await ApiResourceRepository.GetApiResourceSecretNameAsync(apiResourceId);

			await AuditEventLogger.LogEventAsync(new ApiSecretsRequestedEvent(apiSecretsDto.ApiResourceId, apiSecretsDto.ApiResourceSecrets.Select(x => (x.Id, x.Type, x.Expiration)).ToList()));

			return apiSecretsDto;
		}

		public virtual async Task<int> AddApiResourceSecretAsync(ApiResourceSecretsDto apiSecret)
		{
			HashApiSharedSecret(apiSecret);

			var secret = apiSecret.ToEntity();

			var added = await ApiResourceRepository.AddApiResourceSecretAsync(apiSecret.ApiResourceId, secret);

			await AuditEventLogger.LogEventAsync(new ApiSecretAddedEvent(apiSecret.ApiResourceId, apiSecret.Type, apiSecret.Expiration));

			return added;
		}

		public virtual async Task<ApiResourceSecretsDto> GetApiSecretAsync(int apiSecretId)
		{
			var apiSecret = await ApiResourceRepository.GetApiResourceSecretAsync(apiSecretId);
			if (apiSecret == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiSecretDoesNotExist().Description, apiSecretId), ApiResourceServiceResources.ApiSecretDoesNotExist().Description);
			var apiSecretDto = apiSecret.ToModel();

			await AuditEventLogger.LogEventAsync(new ApiSecretRequestedEvent(apiSecretDto.ApiResourceId, apiSecretDto.ApiSecretId, apiSecretDto.Type, apiSecretDto.Expiration));

			return apiSecretDto;
		}

		public virtual async Task<int> DeleteApiResourceSecretAsync(ApiResourceSecretsDto apiSecret)
		{
			var secret = apiSecret.ToEntity();

			var deleted = await ApiResourceRepository.DeleteApiResourceSecretAsync(secret);

			await AuditEventLogger.LogEventAsync(new ApiSecretDeletedEvent(apiSecret.ApiResourceId, apiSecret.ApiSecretId));

			return deleted;
		}

		#endregion
        
        private void HashApiSharedSecret(ApiResourceSecretsDto apiSecret)
        {
            if (apiSecret.Type != SharedSecret) return;

            if (apiSecret.Type == ((int)HashType.Sha256).ToString())
            {
                apiSecret.Value = apiSecret.Value.Sha256();
            }
            else if (apiSecret.Type == ((int)HashType.Sha512).ToString())
            {
                apiSecret.Value = apiSecret.Value.Sha512();
            }
        }

		public virtual async Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await ApiResourceRepository.GetApiResourcesAsync(search, page, pageSize);
            var apiResourcesDto = pagedList.ToModel();

            await AuditEventLogger.LogEventAsync(new ApiResourcesRequestedEvent(apiResourcesDto));

            return apiResourcesDto;
        }

        public virtual async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            return await ApiResourceRepository.GetApiResourceSecretNameAsync(apiResourceId);
        }
	}
}
