using System;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders.Interfaces
{
    public interface IAdminAspNetIdentityDtoConfigurationBuilder<TKey>
        : IAdminAspNetIdentityDtoConfigurationBuilder<TKey, UserDto<TKey>, RoleDto<TKey>, UserClaimsDto<TKey>, UserProviderDto<TKey>, UserProvidersDto<TKey>, UserChangePasswordDto<TKey>, RoleClaimsDto<TKey>, UserClaimDto<TKey>, RoleClaimDto<TKey>, UsersDto<UserDto<TKey>, TKey>, RolesDto<RoleDto<TKey>, TKey>, UserRolesDto<RoleDto<TKey>, TKey>>
        where TKey : IEquatable<TKey>
    {
    }

    public interface IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>
        where TKey : IEquatable<TKey>
        where TUserDto : UserDto<TKey>
        where TRoleDto : RoleDto<TKey>
        where TUserClaimsDto : UserClaimsDto<TKey>
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TKey>
        where TUserClaimDto : UserClaimDto<TKey>
        where TRoleClaimDto : RoleClaimDto<TKey>
        where TUsersDto : UsersDto<TUserDto, TKey>
        where TRolesDto : RolesDto<TRoleDto, TKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
    {
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TNewRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TNewRolesDto, TNewUserRolesDto> UseRole<TNewRoleDto, TNewRolesDto, TNewUserRolesDto>() where TNewRoleDto : RoleDto<TKey> where TNewRolesDto : RolesDto<TNewRoleDto, TKey> where TNewUserRolesDto : UserRolesDto<TNewRoleDto, TKey>;
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TNewRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseRoleClaim<TNewRoleClaimDto>() where TNewRoleClaimDto : RoleClaimDto<TKey>;
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TNewRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseRoleClaims<TNewRoleClaimsDto>() where TNewRoleClaimsDto : RoleClaimsDto<TKey>;
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TNewUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TNewUsersDto, TRolesDto, TUserRolesDto> UseUser<TNewUserDto, TNewUsersDto>() where TNewUserDto : UserDto<TKey> where TNewUsersDto : UsersDto<TNewUserDto, TKey>;
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TNewUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserClaim<TNewUserClaimDto>() where TNewUserClaimDto : UserClaimDto<TKey>;
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TNewUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserClaims<TNewUserClaimsDto>() where TNewUserClaimsDto : UserClaimsDto<TKey>;
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TNewUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserProvider<TNewUserProviderDto>() where TNewUserProviderDto : UserProviderDto<TKey>;
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TNewUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserProviders<TNewUserProvidersDto>() where TNewUserProvidersDto : UserProvidersDto<TKey>;
        IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TNewUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserChangePassword<TNewUserChangePasswordDto>() where TNewUserChangePasswordDto : UserChangePasswordDto<TKey>;
    }
}