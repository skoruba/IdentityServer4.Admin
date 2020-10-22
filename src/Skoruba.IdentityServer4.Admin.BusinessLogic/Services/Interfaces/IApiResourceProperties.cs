using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
	public interface IApiResourceProperties
	{
		Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties);

		Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);

		Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);

		Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId);

		Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);
	}
}