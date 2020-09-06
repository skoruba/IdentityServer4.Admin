using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers.Configuration
{
    public class MapperConfigurationBuilder : IMapperConfigurationBuilder
    {
        public HashSet<Type> ProfileTypes { get; } = new HashSet<Type>();

        public IMapperConfigurationBuilder AddProfilesType(HashSet<Type> profileTypes)
        {
            if (profileTypes == null) return this;

            foreach (var profileType in profileTypes)
            {
                ProfileTypes.Add(profileType);
            }

            return this;
        }

        public IMapperConfigurationBuilder UseIdentityMappingProfile<TUserDto, TRoleDto, TUser, TRole, TKey,
            TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
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
            where TRoleClaimDto : RoleClaimDto<TKey>
        {
            ProfileTypes.Add(typeof(IdentityMapperProfile<TUserDto, TRoleDto, TUser, TRole, TKey,
                TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>));

            return this;
        }
    }
}