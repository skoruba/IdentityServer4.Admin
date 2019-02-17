using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces
{
	public interface IPersistedGrantAspNetIdentityRepository<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>	    
	    where TUser : IdentityUser<TKey>
	    where TRole : IdentityRole<TKey>
	    where TKey : IEquatable<TKey>
	    where TUserClaim : IdentityUserClaim<TKey>
	    where TUserRole : IdentityUserRole<TKey>
	    where TUserLogin : IdentityUserLogin<TKey>
	    where TRoleClaim : IdentityRoleClaim<TKey>
	    where TUserToken : IdentityUserToken<TKey>
    {
		Task<PagedList<PersistedGrantDataView>> GetPersitedGrantsByUsers(string search, int page = 1, int pageSize = 10);
		Task<PagedList<PersistedGrant>> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10);
	    Task<PersistedGrant> GetPersitedGrantAsync(string key);
	    Task<int> DeletePersistedGrantAsync(string key);
	    Task<int> DeletePersistedGrantsAsync(string userId);
        Task<bool> ExistsPersistedGrantsAsync(string subjectId);
	    Task<int> SaveAllChangesAsync();
	}
}