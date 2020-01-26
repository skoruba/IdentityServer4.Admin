using System.Threading.Tasks;
using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Events.IdentityResource;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class IdentityResourceService : IIdentityResourceService
    {
        protected readonly IIdentityResourceRepository IdentityResourceRepository;
        protected readonly IIdentityResourceServiceResources IdentityResourceServiceResources;
        protected readonly IAuditEventLogger AuditEventLogger;

        public IdentityResourceService(IIdentityResourceRepository identityResourceRepository,
            IIdentityResourceServiceResources identityResourceServiceResources,
            IAuditEventLogger auditEventLogger)
        {
            IdentityResourceRepository = identityResourceRepository;
            IdentityResourceServiceResources = identityResourceServiceResources;
            AuditEventLogger = auditEventLogger;
        }

        public virtual async Task<IdentityResourcesDto> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await IdentityResourceRepository.GetIdentityResourcesAsync(search, page, pageSize);
            var identityResourcesDto = pagedList.ToModel();

            await AuditEventLogger.LogEventAsync(new IdentityResourcesRequestedEvent(identityResourcesDto));

            return identityResourcesDto;
        }

        public virtual async Task<IdentityResourceDto> GetIdentityResourceAsync(int identityResourceId)
        {
            var identityResource = await IdentityResourceRepository.GetIdentityResourceAsync(identityResourceId);
            if (identityResource == null) throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description, identityResourceId));

            var identityResourceDto = identityResource.ToModel();

            await AuditEventLogger.LogEventAsync(new IdentityResourceRequestedEvent(identityResourceDto));

            return identityResourceDto;
        }

        public virtual async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1, int pageSize = 10)
        {
            var identityResource = await IdentityResourceRepository.GetIdentityResourceAsync(identityResourceId);
            if (identityResource == null) throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description, identityResourceId), IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description);

            var pagedList = await IdentityResourceRepository.GetIdentityResourcePropertiesAsync(identityResourceId, page, pageSize);
            var identityResourcePropertiesAsync = pagedList.ToModel();
            identityResourcePropertiesAsync.IdentityResourceId = identityResourceId;
            identityResourcePropertiesAsync.IdentityResourceName = identityResource.Name;

            await AuditEventLogger.LogEventAsync(new IdentityResourcePropertiesRequestedEvent(identityResourcePropertiesAsync));

            return identityResourcePropertiesAsync;
        }

        public virtual async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertyAsync(int identityResourcePropertyId)
        {
            var identityResourceProperty = await IdentityResourceRepository.GetIdentityResourcePropertyAsync(identityResourcePropertyId);
            if (identityResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourcePropertyDoesNotExist().Description, identityResourcePropertyId));

            var identityResource = await IdentityResourceRepository.GetIdentityResourceAsync(identityResourceProperty.IdentityResourceId);

            var identityResourcePropertiesDto = identityResourceProperty.ToModel();
            identityResourcePropertiesDto.IdentityResourceId = identityResourceProperty.IdentityResourceId;
            identityResourcePropertiesDto.IdentityResourceName = identityResource.Name;

            await AuditEventLogger.LogEventAsync(new IdentityResourcePropertyRequestedEvent(identityResourcePropertiesDto));

            return identityResourcePropertiesDto;
        }

        public virtual async Task<int> AddIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperties)
        {
            var canInsert = await CanInsertIdentityResourcePropertyAsync(identityResourceProperties);
            if (!canInsert)
            {
                await BuildIdentityResourcePropertiesViewModelAsync(identityResourceProperties);
                throw new UserFriendlyViewException(string.Format(IdentityResourceServiceResources.IdentityResourcePropertyExistsValue().Description, identityResourceProperties.Key), IdentityResourceServiceResources.IdentityResourcePropertyExistsKey().Description, identityResourceProperties);
            }

            var identityResourceProperty = identityResourceProperties.ToEntity();

            var added = await IdentityResourceRepository.AddIdentityResourcePropertyAsync(identityResourceProperties.IdentityResourceId, identityResourceProperty);

            await AuditEventLogger.LogEventAsync(new IdentityResourcePropertyAddedEvent(identityResourceProperties));

            return added;
        }

        private async Task BuildIdentityResourcePropertiesViewModelAsync(IdentityResourcePropertiesDto identityResourceProperties)
        {
            var propertiesDto = await GetIdentityResourcePropertiesAsync(identityResourceProperties.IdentityResourceId);
            identityResourceProperties.IdentityResourceProperties.AddRange(propertiesDto.IdentityResourceProperties);
            identityResourceProperties.TotalCount = propertiesDto.TotalCount;
        }

        public virtual async Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourcePropertiesDto)
        {
            var resource = identityResourcePropertiesDto.ToEntity();

            return await IdentityResourceRepository.CanInsertIdentityResourcePropertyAsync(resource);
        }

        public virtual async Task<int> DeleteIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperty)
        {
            var propertyEntity = identityResourceProperty.ToEntity();

            var deleted = await IdentityResourceRepository.DeleteIdentityResourcePropertyAsync(propertyEntity);

            await AuditEventLogger.LogEventAsync(new IdentityResourcePropertyDeletedEvent(identityResourceProperty));

            return deleted;
        }

        public virtual async Task<bool> CanInsertIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var resource = identityResource.ToEntity();

            return await IdentityResourceRepository.CanInsertIdentityResourceAsync(resource);
        }

        public virtual async Task<int> AddIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var canInsert = await CanInsertIdentityResourceAsync(identityResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(IdentityResourceServiceResources.IdentityResourceExistsValue().Description, identityResource.Name), IdentityResourceServiceResources.IdentityResourceExistsKey().Description, identityResource);
            }

            var resource = identityResource.ToEntity();

            var saved = await IdentityResourceRepository.AddIdentityResourceAsync(resource);

            await AuditEventLogger.LogEventAsync(new IdentityResourceAddedEvent(identityResource));

            return saved;
        }

        public virtual async Task<int> UpdateIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var canInsert = await CanInsertIdentityResourceAsync(identityResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(IdentityResourceServiceResources.IdentityResourceExistsValue().Description, identityResource.Name), IdentityResourceServiceResources.IdentityResourceExistsKey().Description, identityResource);
            }

            var resource = identityResource.ToEntity();

            var originalIdentityResource = await GetIdentityResourceAsync(resource.Id);

            var updated = await IdentityResourceRepository.UpdateIdentityResourceAsync(resource);
            
            await AuditEventLogger.LogEventAsync(new IdentityResourceUpdatedEvent(originalIdentityResource, identityResource));

            return updated;
        }

        public virtual async Task<int> DeleteIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var resource = identityResource.ToEntity();

            var deleted = await IdentityResourceRepository.DeleteIdentityResourceAsync(resource);

            await AuditEventLogger.LogEventAsync(new IdentityResourceDeletedEvent(identityResource));

            return deleted;
        }

        public virtual IdentityResourceDto BuildIdentityResourceViewModel(IdentityResourceDto identityResource)
        {
            ComboBoxHelpers.PopulateValuesToList(identityResource.UserClaimsItems, identityResource.UserClaims);

            return identityResource;
        }
    }
}
