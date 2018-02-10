using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.Data.Entities.Identity;
using Skoruba.IdentityServer4.Admin.ViewModels.Common;

namespace Skoruba.IdentityServer4.Admin.Data.Repositories
{
    public interface IBaseIdentityRepository<in TUserId, in TRoleId, in TClaimId>
    {
        Task<bool> ExistsUserAsync(TUserId userId);

        Task<bool> ExistsRoleAsync(TRoleId roleId);

        Task<PagedList<UserIdentity>> GetUsersAsync(string search, int page = 1, int pageSize = 10);

        Task<PagedList<UserIdentityRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<IdentityResult> CreateRoleAsync(UserIdentityRole role);

        Task<UserIdentityRole> GetRoleAsync(UserIdentityRole role);

        Task<List<UserIdentityRole>> GetRolesAsync();

        Task<IdentityResult> UpdateRoleAsync(UserIdentityRole role);

        Task<UserIdentity> GetUserAsync(UserIdentity user);

        Task<IdentityResult> CreateUserAsync(UserIdentity user);

        Task<IdentityResult> UpdateUserAsync(UserIdentity user);

        Task<IdentityResult> DeleteUserAsync(UserIdentity user);

        Task<IdentityResult> CreateUserRoleAsync(TUserId userId, TRoleId roleId);

        Task<PagedList<UserIdentityRole>> GetUserRolesAsync(TUserId userId, int page = 1, int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(TUserId userId, TRoleId roleId);

        Task<PagedList<UserIdentityUserClaim>> GetUserClaimsAsync(TUserId userId, int page = 1, int pageSize = 10);

        Task<UserIdentityUserClaim> GetUserClaimAsync(TUserId userId, TClaimId claimId);

        Task<IdentityResult> CreateUserClaimsAsync(UserIdentityUserClaim claims);

        Task<int> DeleteUserClaimsAsync(TUserId userId, TClaimId claimId);

        Task<List<UserLoginInfo>> GetUserProvidersAsync(TUserId userId);

        Task<IdentityResult> DeleteUserProvidersAsync(TUserId userId, string providerKey, string loginProvider);

        Task<UserIdentityUserLogin> GetUserProviderAsync(TUserId userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(TUserId userId, string password);

        Task<IdentityResult> CreateRoleClaimsAsync(UserIdentityRoleClaim claims);

        Task<PagedList<UserIdentityRoleClaim>> GetRoleClaimsAsync(TRoleId roleId, int page = 1, int pageSize = 10);

        Task<UserIdentityRoleClaim> GetRoleClaimAsync(TRoleId roleId, TClaimId claimId);

        Task<int> DeleteRoleClaimsAsync(TRoleId roleId, TClaimId claimId);

        Task<IdentityResult> DeleteRoleAsync(UserIdentityRole role);
    }
}