using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IApiResourceService
    {
        ApiSecretsDto BuildApiSecretsViewModel(ApiSecretsDto apiSecrets);

        Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId);

        Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties);

        Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);

        Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);

        Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId);

        Task<int> AddApiResourceAsync(ApiResourceDto apiResource);

        Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource);

        Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource);

        Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource);
        
        Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret);

        Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId);

        Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret);

        Task<string> GetApiResourceNameAsync(int apiResourceId);
    }
}