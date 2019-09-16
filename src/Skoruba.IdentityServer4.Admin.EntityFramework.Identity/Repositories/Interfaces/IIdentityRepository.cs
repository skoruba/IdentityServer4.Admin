using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces
{
	public interface IIdentityRepository<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>	    
	    where TUser : IdentityUser<TKey>
	    where TRole : IdentityRole<TKey>
	    where TKey : IEquatable<TKey>
	    where TUserClaim : IdentityUserClaim<TKey>
	    where TUserRole : IdentityUserRole<TKey>
	    where TUserLogin : IdentityUserLogin<TKey>
	    where TRoleClaim : IdentityRoleClaim<TKey>
	    where TUserToken : IdentityUserToken<TKey>
    {
        Task<bool> ExistsUserAsync(TKey userId);

        Task<bool> ExistsRoleAsync(TKey roleId);

        Task<PagedList<TUser>> GetUsersAsync(string search, int page = 1, int pageSize = 10);

        Task<PagedList<TUser>> GetRoleUsersAsync(TKey roleId, string search, int page = 1, int pageSize = 10);

        Task<PagedList<TRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRole role);

        Task<TRole> GetRoleAsync(TKey roleId);

        Task<List<TRole>> GetRolesAsync();

        Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRole role);

        Task<TUser> GetUserAsync(TKey userId);

        Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUser user);

        Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUser user);

        Task<IdentityResult> DeleteUserAsync(TKey userId);

        Task<IdentityResult> CreateUserRoleAsync(TKey userId, TKey roleId);

        Task<PagedList<TRole>> GetUserRolesAsync(TKey userId, int page = 1, int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(TKey userId, TKey roleId);

        Task<PagedList<TUserClaim>> GetUserClaimsAsync(TKey userId, int page = 1, int pageSize = 10);

        Task<TUserClaim> GetUserClaimAsync(TKey userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(TUserClaim claims);

        Task<int> DeleteUserClaimsAsync(TKey userId, int claimId);

        Task<List<UserLoginInfo>> GetUserProvidersAsync(TKey userId);

        Task<IdentityResult> DeleteUserProvidersAsync(TKey userId, string providerKey, string loginProvider);

        Task<TUserLogin> GetUserProviderAsync(TKey userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(TKey userId, string password);

        Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaim claims);

        Task<PagedList<TRoleClaim>> GetRoleClaimsAsync(TKey roleId, int page = 1, int pageSize = 10);

        Task<TRoleClaim> GetRoleClaimAsync(TKey roleId, int claimId);

        Task<int> DeleteRoleClaimsAsync(TKey roleId, int claimId);

        Task<IdentityResult> DeleteRoleAsync(TRole role);
    }
}