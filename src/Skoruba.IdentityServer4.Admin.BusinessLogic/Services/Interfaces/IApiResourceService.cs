using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IApiResourceService : IApiResourceProperties, IApiScopes, IApiSecrets
    {
        Task<ApiScopesDto> BuildApiScopeViewModelAsync(ApiScopesDto apiScope);

        Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId);

        Task<int> AddApiResourceAsync(ApiResourceDto apiResource);

        Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource);

        Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource);

        Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource);

        Task<string> GetApiResourceNameAsync(int apiResourceId);

		Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10);

	}
}