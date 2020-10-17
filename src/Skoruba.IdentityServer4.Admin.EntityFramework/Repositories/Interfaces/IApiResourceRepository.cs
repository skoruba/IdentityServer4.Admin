using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces
{
    public interface IApiResourceRepository
    {
        Task<PagedList<ApiResource>> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<ApiResource> GetApiResourceAsync(int apiResourceId);

        Task<PagedList<ApiResourceProperty>> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<ApiResourceProperty> GetApiResourcePropertyAsync(int apiResourcePropertyId);

        Task<int> AddApiResourcePropertyAsync(int apiResourceId, ApiResourceProperty apiResourceProperty);

        Task<int> DeleteApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty);

        Task<bool> CanInsertApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty);

        Task<int> AddApiResourceAsync(ApiResource apiResource);

        Task<int> UpdateApiResourceAsync(ApiResource apiResource);

        Task<int> DeleteApiResourceAsync(ApiResource apiResource);

        Task<bool> CanInsertApiResourceAsync(ApiResource apiResource);

        Task<PagedList<ApiScope>> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<ApiScope> GetApiScopeAsync(int apiResourceId, int apiScopeId);

        Task<int> AddApiScopeAsync(int apiResourceId, ApiScope apiScope);

        Task<int> UpdateApiScopeAsync(int apiResourceId, ApiScope apiScope);

        Task<int> DeleteApiScopeAsync(ApiScope apiScope);

        Task<PagedList<ApiSecret>> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<int> AddApiSecretAsync(int apiResourceId, ApiSecret apiSecret);

        Task<ApiSecret> GetApiSecretAsync(int apiSecretId);

        Task<int> DeleteApiSecretAsync(ApiSecret apiSecret);

        Task<bool> CanInsertApiScopeAsync(ApiScope apiScope);

        Task<int> SaveAllChangesAsync();

        bool AutoSaveChanges { get; set; }

        Task<string> GetApiResourceNameAsync(int apiResourceId);
    }
}