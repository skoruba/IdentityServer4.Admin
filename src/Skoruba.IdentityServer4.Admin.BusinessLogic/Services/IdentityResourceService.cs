using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class IdentityResourceService<TDbContext> : IIdentityResourceService<TDbContext>
        where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        private readonly IIdentityResourceRepository<TDbContext> _identityResourceRepository;
        private readonly IIdentityResourceServiceResources _identityResourceServiceResources;

        public IdentityResourceService(IIdentityResourceRepository<TDbContext> identityResourceRepository,
            IIdentityResourceServiceResources identityResourceServiceResources)
        {
            _identityResourceRepository = identityResourceRepository;
            _identityResourceServiceResources = identityResourceServiceResources;
        }

        public async Task<IdentityResourcesDto> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await _identityResourceRepository.GetIdentityResourcesAsync(search, page, pageSize);
            var identityResourcesDto = pagedList.ToModel();

            return identityResourcesDto;
        }

        public async Task<IdentityResourceDto> GetIdentityResourceAsync(int identityResourceId)
        {
            var identityResource = await _identityResourceRepository.GetIdentityResourceAsync(identityResourceId);
            if (identityResource == null) throw new UserFriendlyErrorPageException(string.Format(_identityResourceServiceResources.IdentityResourceDoesNotExist().Description, identityResourceId));

            var identityResourceDto = identityResource.ToModel();

            return identityResourceDto;
        }

        public async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1, int pageSize = 10)
        {
            var identityResource = await _identityResourceRepository.GetIdentityResourceAsync(identityResourceId);
            if (identityResource == null) throw new UserFriendlyErrorPageException(string.Format(_identityResourceServiceResources.IdentityResourceDoesNotExist().Description, identityResourceId), _identityResourceServiceResources.IdentityResourceDoesNotExist().Description);

            var pagedList = await _identityResourceRepository.GetIdentityResourcePropertiesAsync(identityResourceId, page, pageSize);
            var apiResourcePropertiesDto = pagedList.ToModel();
            apiResourcePropertiesDto.IdentityResourceId = identityResourceId;
            apiResourcePropertiesDto.IdentityResourceName = identityResource.Name;

            return apiResourcePropertiesDto;
        }

        public async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertyAsync(int identityResourcePropertyId)
        {
            var identityResourceProperty = await _identityResourceRepository.GetIdentityResourcePropertyAsync(identityResourcePropertyId);
            if (identityResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(_identityResourceServiceResources.IdentityResourcePropertyDoesNotExist().Description, identityResourcePropertyId));

            var identityResource = await _identityResourceRepository.GetIdentityResourceAsync(identityResourceProperty.IdentityResourceId);

            var identityResourcePropertiesDto = identityResourceProperty.ToModel();
            identityResourcePropertiesDto.IdentityResourceId = identityResourceProperty.IdentityResourceId;
            identityResourcePropertiesDto.IdentityResourceName = identityResource.Name;

            return identityResourcePropertiesDto;
        }

        public async Task<int> AddIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperties)
        {
            var canInsert = await CanInsertIdentityResourcePropertyAsync(identityResourceProperties);
            if (!canInsert)
            {
                await BuildIdentityResourcePropertiesViewModelAsync(identityResourceProperties);
                throw new UserFriendlyViewException(string.Format(_identityResourceServiceResources.IdentityResourcePropertyExistsValue().Description, identityResourceProperties.Key), _identityResourceServiceResources.IdentityResourcePropertyExistsKey().Description, identityResourceProperties);
            }

            var identityResourceProperty = identityResourceProperties.ToEntity();

            return await _identityResourceRepository.AddIdentityResourcePropertyAsync(identityResourceProperties.IdentityResourceId, identityResourceProperty);
        }

        private async Task BuildIdentityResourcePropertiesViewModelAsync(IdentityResourcePropertiesDto identityResourceProperties)
        {
            var propertiesDto = await GetIdentityResourcePropertiesAsync(identityResourceProperties.IdentityResourceId);
            identityResourceProperties.IdentityResourceProperties.AddRange(propertiesDto.IdentityResourceProperties);
            identityResourceProperties.TotalCount = propertiesDto.TotalCount;
        }

        public async Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourcePropertiesDto)
        {
            var resource = identityResourcePropertiesDto.ToEntity();

            return await _identityResourceRepository.CanInsertIdentityResourcePropertyAsync(resource);
        }

        public async Task<int> DeleteIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperty)
        {
            var propertyEntity = identityResourceProperty.ToEntity();

            return await _identityResourceRepository.DeleteIdentityResourcePropertyAsync(propertyEntity);
        }

        public async Task<bool> CanInsertIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var resource = identityResource.ToEntity();

            return await _identityResourceRepository.CanInsertIdentityResourceAsync(resource);
        }

        public async Task<int> AddIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var canInsert = await CanInsertIdentityResourceAsync(identityResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(_identityResourceServiceResources.IdentityResourceExistsValue().Description, identityResource.Name), _identityResourceServiceResources.IdentityResourceExistsKey().Description, identityResource);
            }

            var resource = identityResource.ToEntity();

            return await _identityResourceRepository.AddIdentityResourceAsync(resource);
        }

        public async Task<int> UpdateIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var canInsert = await CanInsertIdentityResourceAsync(identityResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(_identityResourceServiceResources.IdentityResourceExistsValue().Description, identityResource.Name), _identityResourceServiceResources.IdentityResourceExistsKey().Description, identityResource);
            }

            var resource = identityResource.ToEntity();

            return await _identityResourceRepository.UpdateIdentityResourceAsync(resource);
        }

        public async Task<int> DeleteIdentityResourceAsync(IdentityResourceDto identityResource)
        {
            var resource = identityResource.ToEntity();

            return await _identityResourceRepository.DeleteIdentityResourceAsync(resource);
        }

        public IdentityResourceDto BuildIdentityResourceViewModel(IdentityResourceDto identityResource)
        {
            ComboBoxHelpers.PopulateValuesToList(identityResource.UserClaimsItems, identityResource.UserClaims);

            return identityResource;
        }
    }
}
