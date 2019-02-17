using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces
{
	public interface IIdentityRepository<TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>	    
	    where TUser : IdentityUser<TKey>
	    where TRole : IdentityRole<TKey>
	    where TKey : IEquatable<TKey>
	    where TUserClaim : IdentityUserClaim<TKey>
	    where TUserRole : IdentityUserRole<TKey>
	    where TUserLogin : IdentityUserLogin<TKey>
	    where TRoleClaim : IdentityRoleClaim<TKey>
	    where TUserToken : IdentityUserToken<TKey>
    {
        Task<bool> ExistsUserAsync(string userId);

        Task<bool> ExistsRoleAsync(string roleId);

        Task<PagedList<TUser>> GetUsersAsync(string search, int page = 1, int pageSize = 10);

        Task<PagedList<TRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRole role);

        Task<TRole> GetRoleAsync(TKey roleId);

        Task<List<TRole>> GetRolesAsync();

        Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRole role);

        Task<TUser> GetUserAsync(string userId);

        Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUser user);

        Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUser user);

        Task<IdentityResult> DeleteUserAsync(string userId);

        Task<IdentityResult> CreateUserRoleAsync(string userId, string roleId);

        Task<PagedList<TRole>> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(string userId, string roleId);

        Task<PagedList<TUserClaim>> GetUserClaimsAsync(string userId, int page = 1, int pageSize = 10);

        Task<TUserClaim> GetUserClaimAsync(string userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(TUserClaim claims);

        Task<int> DeleteUserClaimsAsync(string userId, int claimId);

        Task<List<UserLoginInfo>> GetUserProvidersAsync(string userId);

        Task<IdentityResult> DeleteUserProvidersAsync(string userId, string providerKey, string loginProvider);

        Task<TUserLogin> GetUserProviderAsync(string userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(string userId, string password);

        Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaim claims);

        Task<PagedList<TRoleClaim>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10);

        Task<TRoleClaim> GetRoleClaimAsync(string roleId, int claimId);

        Task<int> DeleteRoleClaimsAsync(string roleId, int claimId);

        Task<IdentityResult> DeleteRoleAsync(TRole role);
    }
}