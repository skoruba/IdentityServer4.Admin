using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
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

        public IdentityResourceService(IIdentityResourceRepository identityResourceRepository,
            IIdentityResourceServiceResources identityResourceServiceResources)
        {
            IdentityResourceRepository = identityResourceRepository;
            IdentityResourceServiceResources = identityResourceServiceResources;
        }

        public virtual async Task<IdentityResourcesDto> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await IdentityResourceRepository.GetIdentityResourcesAsync(search, page, pageSize);
            var identityResourcesDto = pagedList.ToModel();

            return identityResourcesDto;
        }

        public virtual async Task<IdentityResourceDto> GetIdentityResourceAsync(int identityResourceId)
        {
            var identityResource = await IdentityResourceRepository.GetIdentityResourceAsync(identityResourceId);
            if (identityResource == null) throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description, identityResourceId));

            var identityResourceDto = identityResource.ToModel();

            return identityResourceDto;
        }

        public virtual async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1, int pageSize = 10)
        {
            var identityResource = await IdentityResourceRepository.GetIdentityResourceAsync(identityResourceId);
            if (identityResource == null) throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description, identityResourceId), IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description);

            var pagedList = await IdentityResourceRepository.GetIdentityResourcePropertiesAsync(identityResourceId, page, pageSize);
            var apiResourcePropertiesDto = pagedList.ToModel();
            apiResourcePropertiesDto.IdentityResourceId = identityResourceId;
            apiResourcePropertiesDto.IdentityResourceName = identityResource.Name;

            return apiResourcePropertiesDto;
        }

        public virtual async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertyAsync(int identityResourcePropertyId)
        {
            var identityResourceProperty = await IdentityResourceRepository.GetIdentityResourcePropertyAsync(identityResourcePropertyId);
            if (identityResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourcePropertyDoesNotExist().Description, identityResourcePropertyId));

            var identityResource = await IdentityResourceRepository.GetIdentityResourceAsync(identityResourceProperty.IdentityResourceId);

            var identityResourcePropertiesDto = identityResourceProperty.ToModel();
            identityResourcePropertiesDto.IdentityResourceId = identityResourceProperty.IdentityResourceId;
            identityResourcePropertiesDto.IdentityResourceName = identityResource.Name;

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

            return await IdentityResourceRepository.AddIdentityResourcePropertyAsync(identityResourceProperties.IdentityResourceId, identityResourceProperty);
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

            return await IdentityResourceRepository.DeleteIdentityResourcePropertyAsync(propertyEntity);
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

            return await IdentityResourceRepository.AddIdentityResourceAsync(resource);
        }

        public virtual async Task<int> UpdateIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var canInsert = await CanInsertIdentityResourceAsync(identityResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(IdentityResourceServiceResources.IdentityResourceExistsValue().Description, identityResource.Name), IdentityResourceServiceResources.IdentityResourceExistsKey().Description, identityResource);
            }

            var resource = identityResource.ToEntity();

            return await IdentityResourceRepository.UpdateIdentityResourceAsync(resource);
        }

        public virtual async Task<int> DeleteIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var resource = identityResource.ToEntity();

            return await IdentityResourceRepository.DeleteIdentityResourceAsync(resource);
        }

        public virtual IdentityResourceDto BuildIdentityResourceViewModel(IdentityResourceDto identityResource)
        {
            ComboBoxHelpers.PopulateValuesToList(identityResource.UserClaimsItems, identityResource.UserClaims);

            return identityResource;
        }
    }
}
