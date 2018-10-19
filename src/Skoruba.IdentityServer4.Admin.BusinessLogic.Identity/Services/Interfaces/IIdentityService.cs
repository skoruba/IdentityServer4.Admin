using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces
{
    public interface IIdentityService<TIdentityDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUserDto : UserDto<TUserDtoKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRoleDto : RoleDto<TRoleDtoKey>
    {
        Task<bool> ExistsUserAsync(string userId);

        Task<bool> ExistsRoleAsync(string roleId);

        Task<UsersDto<TUserDto, TUserDtoKey>> GetUsersAsync(string search, int page = 1, int pageSize = 10);

        Task<RolesDto<TRoleDto, TRoleDtoKey>> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRoleDto role);

        Task<TRoleDto> GetRoleAsync(string roleId);

        Task<List<TRoleDto>> GetRolesAsync();

        Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRoleDto role);

        Task<TUserDto> GetUserAsync(string userId);

        Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUserDto user);

        Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUserDto user);

        Task<IdentityResult> DeleteUserAsync(string userId, TUserDto user);

        Task<IdentityResult> CreateUserRoleAsync(UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey> role);

        Task<UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>> BuildUserRolesViewModel(TUserDtoKey id, int? page);

        Task<UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>> GetUserRolesAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey> role);

        Task<UserClaimsDto<TUserDtoKey>> GetUserClaimsAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<UserClaimsDto<TUserDtoKey>> GetUserClaimAsync(string userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(UserClaimsDto<TUserDtoKey> claimsDto);

        Task<int> DeleteUserClaimsAsync(UserClaimsDto<TUserDtoKey> claim);

        Task<UserProvidersDto<TUserDtoKey>> GetUserProvidersAsync(string userId);

        TUserDtoKey ConvertUserDtoKeyFromString(string id);

        Task<IdentityResult> DeleteUserProvidersAsync(UserProviderDto<TUserDtoKey> provider);

        Task<UserProviderDto<TUserDtoKey>> GetUserProviderAsync(string userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(UserChangePasswordDto<TUserDtoKey> userPassword);

        Task<IdentityResult> CreateRoleClaimsAsync(RoleClaimsDto<TRoleDtoKey> claimsDto);

        Task<RoleClaimsDto<TRoleDtoKey>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10);

        Task<RoleClaimsDto<TRoleDtoKey>> GetRoleClaimAsync(string roleId, int claimId);

        Task<int> DeleteRoleClaimsAsync(RoleClaimsDto<TRoleDtoKey> role);

        Task<IdentityResult> DeleteRoleAsync(RoleDto<TRoleDtoKey> role);
    }
}