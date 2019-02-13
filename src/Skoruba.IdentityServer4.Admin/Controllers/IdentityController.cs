using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class IdentityController : BaseIdentityController<AdminDbContext, UserDto<string>, string, RoleDto<string>, string, string, string, UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
        UsersDto<UserDto<string>, string>,RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>,string,string>,UserClaimsDto<string>,
        UserProviderDto<string>,UserProvidersDto<string>,UserChangePasswordDto<string>,RoleClaimsDto<string>>
    {
        public IdentityController(IIdentityService<AdminDbContext, UserDto<string>, string, RoleDto<string>, string, string, string, UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,  UserClaimsDto<string>,
                UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>, RoleClaimsDto<string>> identityService, 
            ILogger<ConfigurationController> logger, IStringLocalizer<IdentityController> localizer)
            : base(identityService, logger, localizer)
        {
        }
    }
}