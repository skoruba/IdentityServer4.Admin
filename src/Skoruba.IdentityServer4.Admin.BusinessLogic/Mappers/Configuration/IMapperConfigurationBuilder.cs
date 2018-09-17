using System;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers.Configuration
{
    public interface IMapperConfigurationBuilder
    {
        IMapperConfigurationBuilder UseIdentityMappingProfile<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUser, TRole,
            TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>()
            where TUserDto : UserDto<TUserDtoKey>
            where TRoleDto : RoleDto<TRoleDtoKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>;
    }
}
