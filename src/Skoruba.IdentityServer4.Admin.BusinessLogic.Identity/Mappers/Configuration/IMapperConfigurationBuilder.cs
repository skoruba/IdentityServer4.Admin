using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers.Configuration
{
    public interface IMapperConfigurationBuilder
    {
        HashSet<Type> ProfileTypes { get; }

        IMapperConfigurationBuilder AddProfilesType(HashSet<Type> profileTypes);

        IMapperConfigurationBuilder UseIdentityMappingProfile<TUserDto, TRoleDto, TUser, TRole,
            TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto,
            TUserClaimDto, TRoleClaimDto>()
            where TUserDto : UserDto<TKey>
            where TRoleDto : RoleDto<TKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TUsersDto : UsersDto<TUserDto, TKey>
            where TRolesDto : RolesDto<TRoleDto, TKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
            where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
            where TUserProviderDto : UserProviderDto<TKey>
            where TUserProvidersDto : UserProvidersDto<TKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TKey>
            where TRoleClaimsDto : RoleClaimsDto<TKey>
            where TUserClaimDto : UserClaimDto<TKey>
            where TRoleClaimDto : RoleClaimDto<TKey>;
    }
}
