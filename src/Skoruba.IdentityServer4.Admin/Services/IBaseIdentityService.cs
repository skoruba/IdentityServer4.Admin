using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.ViewModels.Identity;

namespace Skoruba.IdentityServer4.Admin.Services
{
    public interface IBaseIdentityService<in TUserId, in TRoleId, in TClaimId>
    {
        Task<bool> ExistsUserAsync(TUserId userId);

        Task<bool> ExistsRoleAsync(TRoleId roleId);

        Task<UserRolesDto> BuildUserRolesViewModel(TUserId id, int? page);

        Task<UsersDto> GetUsersAsync(string search, int page = 1, int pageSize = 10);

        Task<RolesDto> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<IdentityResult> CreateRoleAsync(RoleDto role);

        Task<RoleDto> GetRoleAsync(RoleDto role);

        Task<List<RoleDto>> GetRolesAsync();

        Task<IdentityResult> UpdateRoleAsync(RoleDto role);

        Task<UserDto> GetUserAsync(UserDto user);

        Task<IdentityResult> CreateUserAsync(UserDto user);

        Task<IdentityResult> UpdateUserAsync(UserDto user);

        Task<IdentityResult> DeleteUserAsync(UserDto user);

        Task<IdentityResult> CreateUserRoleAsync(UserRolesDto role);

        Task<UserRolesDto> GetUserRolesAsync(TUserId userId, int page = 1, int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(UserRolesDto role);

        Task<UserClaimsDto> GetUserClaimsAsync(TUserId userId, int page = 1, int pageSize = 10);

        Task<UserClaimsDto> GetUserClaimAsync(TUserId userId, TClaimId claimId);

        Task<IdentityResult> CreateUserClaimsAsync(UserClaimsDto claimsDto);

        Task<int> DeleteUserClaimsAsync(UserClaimsDto claim);

        Task<UserProvidersDto> GetUserProvidersAsync(TUserId userId);

        Task<IdentityResult> DeleteUserProvidersAsync(UserProviderDto provider);

        Task<UserProviderDto> GetUserProviderAsync(TUserId userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(UserChangePasswordDto userPassword);

        Task<IdentityResult> CreateRoleClaimsAsync(RoleClaimsDto claimsDto);

        Task<RoleClaimsDto> GetRoleClaimsAsync(TRoleId roleId, int page = 1, int pageSize = 10);

        Task<RoleClaimsDto> GetRoleClaimAsync(TRoleId roleId, TClaimId claimId);

        Task<int> DeleteRoleClaimsAsync(RoleClaimsDto claim);

        Task<IdentityResult> DeleteRoleAsync(RoleDto role);
    }
}