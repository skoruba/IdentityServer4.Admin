using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.ViewModels.Configuration;

namespace Skoruba.IdentityServer4.Admin.Services
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
    }
}