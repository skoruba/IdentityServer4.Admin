using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces
{
	public interface IPersistedGrantAspNetIdentityRepository
    {
		Task<PagedList<PersistedGrantDataView>> GetPersistedGrantsByUsers(string search, int page = 1, int pageSize = 10);
		Task<PagedList<PersistedGrant>> GetPersistedGrantsByUser(string subjectId, int page = 1, int pageSize = 10);
	    Task<PersistedGrant> GetPersistedGrantAsync(string key);
	    Task<int> DeletePersistedGrantAsync(string key);
	    Task<int> DeletePersistedGrantsAsync(string userId);
        Task<bool> ExistsPersistedGrantsAsync(string subjectId);
	    Task<int> SaveAllChangesAsync();
	}
}