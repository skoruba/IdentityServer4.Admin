using System.Collections.Generic;
using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IApiScopeService
    {
        ApiScopeDto BuildApiScopeViewModel(ApiScopeDto apiScope);

        Task<ApiScopesDto> GetApiScopesAsync(string search, int page = 1, int pageSize = 10);

        Task<ICollection<string>> GetApiScopesNameAsync(string scope, int limit = 0);

        Task<ApiScopeDto> GetApiScopeAsync(int apiScopeId);

        Task<int> AddApiScopeAsync(ApiScopeDto apiScope);

        Task<int> UpdateApiScopeAsync(ApiScopeDto apiScope);

        Task<int> DeleteApiScopeAsync(ApiScopeDto apiScope);

        Task<bool> CanInsertApiScopeAsync(ApiScopeDto apiScopes);

        Task<ApiScopePropertiesDto> GetApiScopePropertiesAsync(int apiScopeId, int page = 1, int pageSize = 10);

        Task<int> AddApiScopePropertyAsync(ApiScopePropertiesDto apiScopeProperties);

        Task<int> DeleteApiScopePropertyAsync(ApiScopePropertiesDto apiScopeProperty);

        Task<ApiScopePropertiesDto> GetApiScopePropertyAsync(int apiScopePropertyId);

        Task<bool> CanInsertApiScopePropertyAsync(ApiScopePropertiesDto apiResourceProperty);
    }
}