using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.ExceptionHandling;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public static class IdentityMappers<TIdentityMappingProfile> where TIdentityMappingProfile : Profile, new()
    {
        internal static IMapper Mapper { get; }

        static IdentityMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<TIdentityMappingProfile>())
                .CreateMapper();
        }

        public static UsersDto<TUserDto, TUserDtoKey> ToUserModel<TUser, TKey, TUserDto, TUserDtoKey>(PagedList<TUser> users)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
            where TUserDto : UserDto<TUserDtoKey>
        {
            return Mapper.Map<UsersDto<TUserDto, TUserDtoKey>>(users);
        }

        public static RolesDto<TRoleDto, TRoleDtoKey> ToRoleModel<TRole, TKey, TRoleDto, TRoleDtoKey>(PagedList<TRole> roles)
            where TRoleDto : RoleDto<TRoleDtoKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            return Mapper.Map<RolesDto<TRoleDto, TRoleDtoKey>>(roles);
        }

        public static TRole ToRoleEntity<TRoleDto, TRoleDtoKey, TRole, TKey>(TRoleDto role)
            where TRole : IdentityRole<TKey> where TKey : IEquatable<TKey>
            where TRoleDto : RoleDto<TRoleDtoKey>
        {
            return Mapper.Map<TRole>(role);
        }

        public static List<ViewErrorMessage> ToViewErrorMessages(IEnumerable<IdentityError> errors)
        {
            return Mapper.Map<List<ViewErrorMessage>>(errors);
        }

        public static TRoleDto ToRoleModel<TRoleDto, TRoleDtoKey, TRole, TKey>(TRole role)
            where TRoleDto : RoleDto<TRoleDtoKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            return Mapper.Map<TRoleDto>(role);
        }

        public static List<TRoleDto> ToRoleModel<TRoleDto, TRoleDtoKey, TRole, TKey>(List<TRole> roles)
            where TRoleDto : RoleDto<TRoleDtoKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            return Mapper.Map<List<TRoleDto>>(roles);
        }

        public static TUser ToUserEntity<TUserDto, TUserDtoKey, TUser, TKey>(TUserDto user)
            where TUserDto : UserDto<TUserDtoKey>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return Mapper.Map<TUser>(user);
        }

        public static TUserDto ToUserModel<TUser, TKey, TUserDto, TUserDtoKey>(TUser user)
            where TUserDto : UserDto<TUserDtoKey>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return Mapper.Map<TUserDto>(user);
        }

        public static UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey> ToUserRoleModel<TRole, TKey, TRoleDto, TRoleDtoKey, TUserDtoKey>(PagedList<TRole> roles)
            where TRoleDto : RoleDto<TRoleDtoKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            return Mapper.Map<UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>>(roles);
        }

        public static UserClaimsDto<TUserDtoKey, TClaimDtoKey> ToUserClaimModel<TUserClaim, TUserDtoKey, TClaimDtoKey, TKey>(PagedList<TUserClaim> userClaim)
            where TUserClaim : IdentityUserClaim<TKey>
            where TKey : IEquatable<TKey>
        {
            return Mapper.Map<UserClaimsDto<TUserDtoKey, TClaimDtoKey>>(userClaim);
        }
    }
}
