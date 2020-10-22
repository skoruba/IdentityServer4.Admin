using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
	public interface IApiSecrets
	{
		Task<int> AddApiResourceSecretAsync(ApiResourceSecretsDto apiSecret);

		Task<ApiResourceSecretsDto> GetApiSecretAsync(int apiSecretId);

		Task<int> DeleteApiResourceSecretAsync(ApiResourceSecretsDto apiSecret);

		ApiResourceSecretsDto BuildApiSecretsViewModel(ApiResourceSecretsDto apiSecrets);

		Task<ApiResourceSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10);
	}
}