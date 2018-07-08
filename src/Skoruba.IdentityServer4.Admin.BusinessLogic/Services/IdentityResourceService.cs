using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class IdentityResourceService : IIdentityResourceService
    {
        private readonly IIdentityResourceRepository _identityResourceRepository;
        private readonly IIdentityResourceServiceResources _identityResourceServiceResources;

        public IdentityResourceService(IIdentityResourceRepository identityResourceRepository,
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
