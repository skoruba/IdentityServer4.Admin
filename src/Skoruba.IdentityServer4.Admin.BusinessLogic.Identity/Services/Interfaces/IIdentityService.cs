using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces
{
    public interface IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, 
        TUserLogin, TRoleClaim, TUserToken, 
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>        
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
        where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
        where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
        where TUserClaimsDto: UserClaimsDto<TUserDtoKey>
        where TUserProviderDto : UserProviderDto<TUserDtoKey>
        where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
        where TUserChangePasswordDto: UserChangePasswordDto<TUserDtoKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
    {
        Task<bool> ExistsUserAsync(string userId);

        Task<bool> ExistsRoleAsync(string roleId);

        Task<TUsersDto> GetUsersAsync(string search, int page = 1, int pageSize = 10);

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

        Task<TUserRolesDto> BuildUserRolesViewModel(TUserDtoKey id, int? page);

        Task<TUserRolesDto> GetUserRolesAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(TUserRolesDto role);

        Task<TUserClaimsDto> GetUserClaimsAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<TUserClaimsDto> GetUserClaimAsync(string userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(TUserClaimsDto claimsDto);

        Task<int> DeleteUserClaimsAsync(TUserClaimsDto claim);

        Task<TUserProvidersDto> GetUserProvidersAsync(string userId);

        TUserDtoKey ConvertUserDtoKeyFromString(string id);

        Task<IdentityResult> DeleteUserProvidersAsync(TUserProviderDto provider);

        Task<TUserProviderDto> GetUserProviderAsync(string userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(TUserChangePasswordDto userPassword);

        Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaimsDto claimsDto);

        Task<TRoleClaimsDto> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10);

        Task<TRoleClaimsDto> GetRoleClaimAsync(string roleId, int claimId);

        Task<int> DeleteRoleClaimsAsync(TRoleClaimsDto role);

        Task<IdentityResult> DeleteRoleAsync(TRoleDto role);
    }
}