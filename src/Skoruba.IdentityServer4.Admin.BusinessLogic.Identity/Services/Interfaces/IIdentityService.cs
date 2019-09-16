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
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>        
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
        where TUserClaimsDto: UserClaimsDto<TKey>
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TKey>
        where TUserChangePasswordDto: UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TKey>
    {
        Task<bool> ExistsUserAsync(TKey userId);

        Task<bool> ExistsRoleAsync(TKey roleId);

        Task<TUsersDto> GetUsersAsync(string search, int page = 1, int pageSize = 10);
        Task<TUsersDto> GetRoleUsersAsync(TKey roleId, string search, int page = 1, int pageSize = 10);
        Task<TRolesDto> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRoleDto role);

        Task<TRoleDto> GetRoleAsync(TKey roleId);

        Task<List<TRoleDto>> GetRolesAsync();

        Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRoleDto role);

        Task<TUserDto> GetUserAsync(TKey userId);

        Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUserDto user);

        Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUserDto user);

        Task<IdentityResult> DeleteUserAsync(TKey userId, TUserDto user);

        Task<IdentityResult> CreateUserRoleAsync(TUserRolesDto role);

        Task<TUserRolesDto> BuildUserRolesViewModel(TKey id, int? page);

        Task<TUserRolesDto> GetUserRolesAsync(TKey userId, int page = 1,
            int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(TUserRolesDto role);

        Task<TUserClaimsDto> GetUserClaimsAsync(TKey userId, int page = 1,
            int pageSize = 10);

        Task<TUserClaimsDto> GetUserClaimAsync(TKey userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(TUserClaimsDto claimsDto);

        Task<int> DeleteUserClaimsAsync(TUserClaimsDto claim);

        Task<TUserProvidersDto> GetUserProvidersAsync(TKey userId);

        TKey ConvertUserDtoKeyFromString(string id);

        Task<IdentityResult> DeleteUserProvidersAsync(TUserProviderDto provider);

        Task<TUserProviderDto> GetUserProviderAsync(TKey userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(TUserChangePasswordDto userPassword);

        Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaimsDto claimsDto);

        Task<TRoleClaimsDto> GetRoleClaimsAsync(TKey roleId, int page = 1, int pageSize = 10);

        Task<TRoleClaimsDto> GetRoleClaimAsync(TKey roleId, int claimId);

        Task<int> DeleteRoleClaimsAsync(TRoleClaimsDto role);

        Task<IdentityResult> DeleteRoleAsync(TRoleDto role);
    }
}