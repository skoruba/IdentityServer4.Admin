using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories
{
	public interface IIdentityResourceRepository
	{
		Task<PagedList<IdentityResource>> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10);

		Task<IdentityResource> GetIdentityResourceAsync(int identityResourceId);

        Task<bool> CanInsertIdentityResourceAsync(IdentityResource identityResource);

        Task<int> AddIdentityResourceAsync(IdentityResource identityResource);

		Task<int> UpdateIdentityResourceAsync(IdentityResource identityResource);

		Task<int> DeleteIdentityResourceAsync(IdentityResource identityResource);

	    Task<int> SaveAllChangesAsync();
	}
}