using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
	public interface IApiScopes
	{
		Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes);

		Task<int> AddApiScopeAsync(int apiResourceId, ApiScopesDto apiScope);

		Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope);

		Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope);

		Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId);

		Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10);
	}
}