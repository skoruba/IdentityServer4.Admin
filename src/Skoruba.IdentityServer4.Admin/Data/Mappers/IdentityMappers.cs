using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities.Identity;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.ViewModels.Common;
using Skoruba.IdentityServer4.Admin.ViewModels.Identity;

namespace Skoruba.IdentityServer4.Admin.Data.Mappers
{
    public static class IdentityMappers
    {
        internal static IMapper Mapper { get; }

        static IdentityMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityMapperProfile>())
                .CreateMapper();
        }

        public static UsersDto ToModel(this PagedList<UserIdentity> users)
        {
            return Mapper.Map<UsersDto>(users);
        }

        public static UserClaimsDto ToModel(this PagedList<UserIdentityUserClaim> claims)
        {
            return Mapper.Map<UserClaimsDto>(claims);
        }

        public static UserClaimsDto ToModel(this UserIdentityUserClaim claim)
        {
            return Mapper.Map<UserClaimsDto>(claim);
        }

        public static RolesDto ToModel(this PagedList<UserIdentityRole> roles)
        {
            return Mapper.Map<RolesDto>(roles);
        }

        public static UserRolesDto MapToModel(this PagedList<UserIdentityRole> roles)
        {
            return Mapper.Map<UserRolesDto>(roles);
        }

        public static UserDto ToModel(this UserIdentity user)
        {
            return Mapper.Map<UserDto>(user);
        }

        public static List<ViewErrorMessage> ToModel(this IEnumerable<IdentityError> error)
        {
            return Mapper.Map<List<ViewErrorMessage>>(error);
        }

        public static RoleDto ToModel(this UserIdentityRole role)
        {
            return Mapper.Map<RoleDto>(role);
        }

        public static UserProviderDto ToModel(this UserIdentityUserLogin login)
        {
            return Mapper.Map<UserProviderDto>(login);
        }

        public static UserProvidersDto ToModel(this List<UserLoginInfo> login)
        {
            return Mapper.Map<UserProvidersDto>(login);
        }

        public static RoleClaimsDto ToModel(this UserIdentityRoleClaim roleClaim)
        {
            return Mapper.Map<RoleClaimsDto>(roleClaim);
        }

        public static List<RoleDto> ToModel(this List<UserIdentityRole> roles)
        {
            return Mapper.Map<List<RoleDto>>(roles);
        }

        public static RoleClaimsDto ToModel(this PagedList<UserIdentityRoleClaim> roleClaim)
        {
            return Mapper.Map<RoleClaimsDto>(roleClaim);
        }
        
        public static UserIdentityUserClaim ToEntity(this UserClaimsDto claim)
        {
            return Mapper.Map<UserIdentityUserClaim>(claim);
        }

        public static UserIdentity ToEntity(this UserDto user)
        {
            return Mapper.Map<UserIdentity>(user);
        }

        public static UserIdentityRoleClaim ToEntity(this RoleClaimsDto roleClaim)
        {
            return Mapper.Map<UserIdentityRoleClaim>(roleClaim);
        }

        public static UserIdentityRole ToEntity(this RoleDto role)
        {
            return Mapper.Map<UserIdentityRole>(role);
        }

        public static UserIdentity MapTo(this UserIdentity user, UserIdentity newUser)
        {
            return Mapper.Map(newUser, user);
        }
    }
}
