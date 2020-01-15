using System;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders
{
    internal class AdminAspNetIdentityDtoConfigurationBuilder<TKey> : AdminAspNetIdentityDtoConfigurationBuilder<TKey, UserDto<TKey>, RoleDto<TKey>, UserClaimsDto<TKey>, UserProviderDto<TKey>, UserProvidersDto<TKey>, UserChangePasswordDto<TKey>, RoleClaimsDto<TKey>, UserClaimDto<TKey>, RoleClaimDto<TKey>, UsersDto<UserDto<TKey>, TKey>, RolesDto<RoleDto<TKey>, TKey>, UserRolesDto<RoleDto<TKey>, TKey>>, IAdminAspNetIdentityDtoConfigurationBuilder<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    internal class AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> : IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>
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
        public AdminAspNetIdentityConfiguration<TKey>.Dto Build() => new AdminAspNetIdentityConfiguration<TKey>.Dto
        {
            User = typeof(TUserDto),
            Role = typeof(TRoleDto),
            UserClaims = typeof(TUserClaimsDto),
            UserProvider = typeof(TUserProviderDto),
            UserProviders = typeof(TUserProvidersDto),
            UserChangePassword = typeof(TUserChangePasswordDto),
            RoleClaims = typeof(TRoleClaimsDto),
            UserClaim = typeof(TUserClaimDto),
            RoleClaim = typeof(TRoleClaimDto),
            Users = typeof(TUsersDto),
            Roles = typeof(TRolesDto),
            UserRoles = typeof(TUserRolesDto)
        };

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TNewRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TNewRolesDto, TNewUserRolesDto> UseRole<TNewRoleDto, TNewRolesDto, TNewUserRolesDto>()
            where TNewRoleDto : RoleDto<TKey>
            where TNewRolesDto : RolesDto<TNewRoleDto, TKey>
            where TNewUserRolesDto : UserRolesDto<TNewRoleDto, TKey> =>
            new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TNewRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TNewRolesDto, TNewUserRolesDto>();

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TNewRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseRoleClaim<TNewRoleClaimDto>() where TNewRoleClaimDto : RoleClaimDto<TKey> =>
            new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TNewRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>();

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TNewRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseRoleClaims<TNewRoleClaimsDto>() where TNewRoleClaimsDto : RoleClaimsDto<TKey> =>
            new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TNewRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>();

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TNewUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TNewUsersDto, TRolesDto, TUserRolesDto> UseUser<TNewUserDto, TNewUsersDto>()
            where TNewUserDto : UserDto<TKey>
            where TNewUsersDto : UsersDto<TNewUserDto, TKey> =>
                new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TNewUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TNewUsersDto, TRolesDto, TUserRolesDto>();

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TNewUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserChangePassword<TNewUserChangePasswordDto>() where TNewUserChangePasswordDto : UserChangePasswordDto<TKey> =>
            new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TNewUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>();

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TNewUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserClaim<TNewUserClaimDto>() where TNewUserClaimDto : UserClaimDto<TKey> =>
            new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TNewUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>();

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TNewUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserClaims<TNewUserClaimsDto>() where TNewUserClaimsDto : UserClaimsDto<TKey> =>
            new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TNewUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>();

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TNewUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserProvider<TNewUserProviderDto>() where TNewUserProviderDto : UserProviderDto<TKey> =>
            new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TNewUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>();

        public IAdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TNewUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto> UseUserProviders<TNewUserProvidersDto>() where TNewUserProvidersDto : UserProvidersDto<TKey> =>
            new AdminAspNetIdentityDtoConfigurationBuilder<TKey, TUserDto, TRoleDto, TUserClaimsDto, TUserProviderDto, TNewUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto, TUsersDto, TRolesDto, TUserRolesDto>();
    }
}