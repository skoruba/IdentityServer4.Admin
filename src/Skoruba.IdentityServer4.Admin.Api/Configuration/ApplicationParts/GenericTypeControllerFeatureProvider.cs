using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.Api.Configuration.ApplicationParts
{
    public class GenericTypeControllerFeatureProvider<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> : IApplicationFeatureProvider<ControllerFeature>
        where TUserDto : UserDto<TUserDtoKey>, new()
        where TRoleDto : RoleDto<TRoleDtoKey>, new()
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRoleDtoKey : IEquatable<TRoleDtoKey>
        where TUserDtoKey : IEquatable<TUserDtoKey>
        where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
        where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
        where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
        where TUserProviderDto : UserProviderDto<TUserDtoKey>
        where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var currentAssembly = typeof(GenericTypeControllerFeatureProvider<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>).Assembly;
            var controllerTypes = currentAssembly.GetExportedTypes()
                                                 .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && t.IsGenericTypeDefinition)
                                                 .Select(t => t.GetTypeInfo());

            var type = this.GetType();
            var genericType = type.GetGenericTypeDefinition().GetTypeInfo();
            var parameters = genericType.GenericTypeParameters
                                        .Select((p, i) => new { p.Name, Index = i })
                                        .ToDictionary(a => a.Name, a => type.GenericTypeArguments[a.Index]);

            foreach (var controllerType in controllerTypes)
            {
                var typeArguments = controllerType.GenericTypeParameters
                                                  .Select(p => parameters[p.Name])
                                                  .ToArray();

                feature.Controllers.Add(controllerType.MakeGenericType(typeArguments).GetTypeInfo());
            }
        }
    }
}