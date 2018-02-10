using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.Data.Entities;
using Skoruba.IdentityServer4.Admin.ViewModels.Common;

namespace Skoruba.IdentityServer4.Admin.Data.Repositories
{
	public interface IPersistedGrantRepository
	{
		Task<PagedList<PersistedGrantDataView>> GetPersitedGrantsByUsers(int page = 1, int pageSize = 10);
		Task<PagedList<PersistedGrant>> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10);
	    Task<PersistedGrant> GetPersitedGrantAsync(string key);
	    Task<int> DeletePersistedGrantAsync(string key);
	    Task<int> DeletePersistedGrantsAsync(int userId);
        Task<bool> ExistsPersistedGrantsAsync(string subjectId);
	    Task<int> SaveAllChangesAsync();
	}
}