using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.Api.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Api.Helpers.Localization;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json")]
    public class RolesController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> : ControllerBase
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
        where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>, new()
    {
        private readonly IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> _identityService;
        private readonly IGenericControllerLocalizer<UsersController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>> _localizer;

        public RolesController(IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> identityService,
            IGenericControllerLocalizer<UsersController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>> localizer)
        {
            _identityService = identityService;
            _localizer = localizer;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TRoleDto>> Get(TUserDtoKey id)
        {
            var role = await _identityService.GetRoleAsync(id.ToString());

            return Ok(role);
        }

        [HttpGet]
        public async Task<ActionResult<TRolesDto>> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var rolesDto = await _identityService.GetRolesAsync(searchText, page, pageSize);

            return Ok(rolesDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TRoleDto role)
        {
            await _identityService.CreateRoleAsync(role);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]TRoleDto role)
        {
            await _identityService.UpdateRoleAsync(role);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(TRoleDtoKey id)
        {
            var roleDto = new TRoleDto { Id = id };

            await _identityService.DeleteRoleAsync(roleDto);

            return Ok();
        }

        [HttpGet("{id}/Users")]
        public async Task<ActionResult<TRolesDto>> GetRoleUsers(string id, string searchText, int page = 1, int pageSize = 10)
        {
            var usersDto = await _identityService.GetRoleUsersAsync(id, searchText, page, pageSize);

            return Ok(usersDto);
        }

        [HttpGet("{id}/Claims")]
        public async Task<ActionResult<TRoleClaimsDto>> GetRoleClaims(string id, int page = 1, int pageSize = 10)
        {
            var roleClaimsDto = await _identityService.GetRoleClaimsAsync(id, page, pageSize);

            return Ok(roleClaimsDto);
        }

        [HttpPost("Claims")]
        public async Task<IActionResult> PostRoleClaims([FromBody]TRoleClaimsDto roleClaims)
        {
            await _identityService.CreateRoleClaimsAsync(roleClaims);

            return Ok();
        }

        [HttpDelete("{id}/Claims")]
        public async Task<IActionResult> DeleteRoleClaims(TRoleDtoKey id, int claimId)
        {
            var roleDto = new TRoleClaimsDto { ClaimId = claimId, RoleId = id };

            await _identityService.DeleteRoleClaimsAsync(roleDto);

            return Ok();
        }
    }
}