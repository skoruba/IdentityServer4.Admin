using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public interface IApiResourceService
    {
        ApiSecretsDto BuildApiSecretsViewModel(ApiSecretsDto apiSecrets);

        ApiScopesDto BuildApiScopeViewModel(ApiScopesDto apiScope);

        Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId);

        Task<int> AddApiResourceAsync(ApiResourceDto apiResource);

        Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource);

        Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource);

        Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource);

        Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId);

        Task<int> AddApiScopeAsync(ApiScopesDto apiScope);

        Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope);

        Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope);

        Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret);

        Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId);

        Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret);

        Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes);

        Task<string> GetApiResourceNameAsync(int apiResourceId);
    }
}