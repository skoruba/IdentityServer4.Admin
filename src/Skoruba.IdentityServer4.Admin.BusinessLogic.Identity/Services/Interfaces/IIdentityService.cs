using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces
{
    public interface IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole,
        TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto>
        where TUserDto : UserDto<TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRoleDto : RoleDto<TKey>
        where TUsersDto : UsersDto<TUserDto, TKey>
        where TRolesDto : RolesDto<TRoleDto, TKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TKey>
        where TUserClaimDto : UserClaimDto<TKey>
    {
        Task<bool> ExistsUserAsync(string userId);

        Task<bool> ExistsRoleAsync(string roleId);

        Task<TUsersDto> GetUsersAsync(string search, int page = 1, int pageSize = 10);
        Task<TUsersDto> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10);
        Task<TUsersDto> GetClaimUsersAsync(string claimType, string claimValue, int page = 1, int pageSize = 10);

        Task<TRolesDto> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRoleDto role);

        Task<TRoleDto> GetRoleAsync(string roleId);

        Task<List<TRoleDto>> GetRolesAsync();

        Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRoleDto role);

        Task<TUserDto> GetUserAsync(string userId);

        Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUserDto user);

        Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUserDto user);

        Task<IdentityResult> DeleteUserAsync(string userId, TUserDto user);

        Task<IdentityResult> CreateUserRoleAsync(TUserRolesDto role);

        Task<TUserRolesDto> BuildUserRolesViewModel(TKey id, int? page);

        Task<TUserRolesDto> GetUserRolesAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(TUserRolesDto role);

        Task<TUserClaimsDto> GetUserClaimsAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<TUserClaimsDto> GetUserClaimAsync(string userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(TUserClaimsDto claimsDto);

        Task<IdentityResult> UpdateUserClaimsAsync(TUserClaimsDto claimsDto);

        Task<IdentityResult> DeleteUserClaimAsync(TUserClaimsDto claim);

        Task<TUserProvidersDto> GetUserProvidersAsync(string userId);

        TKey ConvertToKeyFromString(string id);

        Task<IdentityResult> DeleteUserProvidersAsync(TUserProviderDto provider);

        Task<TUserProviderDto> GetUserProviderAsync(string userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(TUserChangePasswordDto userPassword);

        Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaimsDto claimsDto);

        Task<IdentityResult> UpdateRoleClaimsAsync(TRoleClaimsDto claimsDto);

        Task<TRoleClaimsDto> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10);

        Task<TRoleClaimsDto> GetUserRoleClaimsAsync(string userId, string claimSearchText, int page = 1, int pageSize = 10);

        Task<TRoleClaimsDto> GetRoleClaimAsync(string roleId, int claimId);

        Task<IdentityResult> DeleteRoleClaimAsync(TRoleClaimsDto role);

        Task<IdentityResult> DeleteRoleAsync(TRoleDto role);

        Task<string> GeneratePasswordResetTokenAsync(TUserDto user);
    }
}