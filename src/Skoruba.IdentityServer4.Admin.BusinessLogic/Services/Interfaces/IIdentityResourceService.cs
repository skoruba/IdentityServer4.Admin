using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IIdentityResourceService
    {
        Task<IdentityResourcesDto> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<IdentityResourceDto> GetIdentityResourceAsync(int identityResourceId);

        Task<bool> CanInsertIdentityResourceAsync(IdentityResourceDto identityResource);

        Task<int> AddIdentityResourceAsync(IdentityResourceDto identityResource);

        Task<int> UpdateIdentityResourceAsync(IdentityResourceDto identityResource);

        Task<int> DeleteIdentityResourceAsync(IdentityResourceDto identityResource);

        IdentityResourceDto BuildIdentityResourceViewModel(IdentityResourceDto identityResource);

        Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1,
            int pageSize = 10);

        Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertyAsync(int identityResourcePropertyId);

        Task<int> AddIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperties);

        Task<int> DeleteIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperty);

        Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourcePropertiesDto);
    }
}