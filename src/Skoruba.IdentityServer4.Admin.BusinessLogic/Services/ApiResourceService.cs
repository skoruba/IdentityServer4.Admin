using System.Linq;
using System.Threading.Tasks;
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

        public virtual async Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await ApiResourceRepository.GetApiResourcesAsync(search, page, pageSize);
            var apiResourcesDto = pagedList.ToModel();

            await AuditEventLogger.LogEventAsync(new ApiResourcesRequestedEvent(apiResourcesDto));

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

            await AuditEventLogger.LogEventAsync(new ApiResourcePropertiesRequestedEvent(apiResourceId, apiResourcePropertiesDto));

            return apiResourcePropertiesDto;
        }

        public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId)
        {
            var apiResourceProperty = await ApiResourceRepository.GetApiResourcePropertyAsync(apiResourcePropertyId);
            if (apiResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourcePropertyDoesNotExist().Description, apiResourcePropertyId));

            var apiResourcePropertiesDto = apiResourceProperty.ToModel();
            apiResourcePropertiesDto.ApiResourceId = apiResourceProperty.ApiResourceId;
            apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceProperty.ApiResourceId);

            await AuditEventLogger.LogEventAsync(new ApiResourcePropertyRequestedEvent(apiResourcePropertyId, apiResourcePropertiesDto));

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

            await AuditEventLogger.LogEventAsync(new ApiResourcePropertyAddedEvent(apiResourceProperties));

            return saved;
        }

        public virtual async Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
        {
            var propertyEntity = apiResourceProperty.ToEntity();

            var deleted = await ApiResourceRepository.DeleteApiResourcePropertyAsync(propertyEntity);

            await AuditEventLogger.LogEventAsync(new ApiResourcePropertyDeletedEvent(apiResourceProperty));

            return deleted;
        }

        public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
        {
            var resource = apiResourceProperty.ToEntity();

            return await ApiResourceRepository.CanInsertApiResourcePropertyAsync(resource);
        }

        private void HashApiSharedSecret(ApiSecretsDto apiSecret)
        {
            if (apiSecret.Type != SharedSecret) return;

            if (apiSecret.HashTypeEnum == HashType.Sha256)
            {
                apiSecret.Value = apiSecret.Value.Sha256();
            }
            else if (apiSecret.HashTypeEnum == HashType.Sha512)
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

        private async Task BuildApiResourcePropertiesViewModelAsync(ApiResourcePropertiesDto apiResourceProperties)
        {
            var apiResourcePropertiesDto = await GetApiResourcePropertiesAsync(apiResourceProperties.ApiResourceId);
            apiResourceProperties.ApiResourceProperties.AddRange(apiResourcePropertiesDto.ApiResourceProperties);
            apiResourceProperties.TotalCount = apiResourcePropertiesDto.TotalCount;
        }

        public virtual async Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await ApiResourceRepository.GetApiSecretsAsync(apiResourceId, page, pageSize);

            var apiSecretsDto = pagedList.ToModel();
            apiSecretsDto.ApiResourceId = apiResourceId;
            apiSecretsDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);

            // remove secret value from dto
            apiSecretsDto.ApiSecrets.ForEach(x => x.Value = null);

            await AuditEventLogger.LogEventAsync(new ApiSecretsRequestedEvent(apiSecretsDto.ApiResourceId, apiSecretsDto.ApiSecrets.Select(x => (x.Id, x.Type, x.Expiration)).ToList()));

            return apiSecretsDto;
        }

        public virtual async Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret)
        {
            HashApiSharedSecret(apiSecret);

            var secret = apiSecret.ToEntity();

            var added = await ApiResourceRepository.AddApiSecretAsync(apiSecret.ApiResourceId, secret);

            await AuditEventLogger.LogEventAsync(new ApiSecretAddedEvent(apiSecret.ApiResourceId, apiSecret.Type, apiSecret.Expiration));

            return added;
        }

        public virtual async Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId)
        {
            var apiSecret = await ApiResourceRepository.GetApiSecretAsync(apiSecretId);
            if (apiSecret == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiSecretDoesNotExist().Description, apiSecretId), ApiResourceServiceResources.ApiSecretDoesNotExist().Description);
            var apiSecretsDto = apiSecret.ToModel();

            // remove secret value for dto
            apiSecretsDto.Value = null;

            await AuditEventLogger.LogEventAsync(new ApiSecretRequestedEvent(apiSecretsDto.ApiResourceId, apiSecretsDto.ApiSecretId, apiSecretsDto.Type, apiSecretsDto.Expiration));

            return apiSecretsDto;
        }

        public virtual async Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret)
        {
            var secret = apiSecret.ToEntity();

            var deleted = await ApiResourceRepository.DeleteApiSecretAsync(secret);

            await AuditEventLogger.LogEventAsync(new ApiSecretDeletedEvent(apiSecret.ApiResourceId, apiSecret.ApiSecretId));

            return deleted;
        }

        public virtual async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            return await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);
        }
    }
}
