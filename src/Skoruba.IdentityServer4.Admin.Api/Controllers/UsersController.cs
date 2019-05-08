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
    public class UsersController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
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
        where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
    {
        private readonly IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> _identityService;
        private readonly IGenericControllerLocalizer<UsersController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>> _localizer;

        public UsersController(IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
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
        public async Task<ActionResult<TUserDto>> Get(TUserDtoKey id)
        {
            var user = await _identityService.GetUserAsync(id.ToString());
           
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<TUsersDto>> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var usersDto = await _identityService.GetUsersAsync(searchText, page, pageSize);

            return Ok(usersDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TUserDto user)
        {
            await _identityService.CreateUserAsync(user);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody]TUserDto user)
        {
            await _identityService.UpdateUserAsync(user);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(TUserDtoKey id)
        {
            var user = new TUserDto { Id = id };

            await _identityService.DeleteUserAsync(user.Id.ToString(), user);

            return Ok();
        }

        [HttpGet("{id}/Roles")]
        public async Task<ActionResult<TUserRolesDto>> GetUserRoles(string id, int page = 1, int pageSize = 10)
        {
            var userRoles = await _identityService.GetUserRolesAsync(id, page, pageSize);

            return Ok(userRoles);
        }

        [HttpPost("Roles")]
        public async Task<IActionResult> PostUserRoles([FromBody]TUserRolesDto role)
        {
            await _identityService.CreateUserRoleAsync(role);

            return Ok();
        }

        [HttpDelete("Roles")]
        public async Task<IActionResult> DeleteUserRoles([FromBody]TUserRolesDto role)
        {
            await _identityService.DeleteUserRoleAsync(role);

            return Ok();
        }

        [HttpGet("{id}/Claims")]
        public async Task<ActionResult<TUserClaimsDto>> GetUserClaims(TUserDtoKey id, int page = 1, int pageSize = 10)
        {
            var claims = await _identityService.GetUserClaimsAsync(id.ToString(), page, pageSize);

            return Ok(claims);
        }

        [HttpPost("Claims")]
        public async Task<IActionResult> PostUserClaims([FromBody]TUserClaimsDto claim)
        {
            await _identityService.CreateUserClaimsAsync(claim);

            return Ok();
        }

        [HttpDelete("Claims")]
        public async Task<IActionResult> DeleteUserClaims([FromBody]TUserClaimsDto claim)
        {
            await _identityService.DeleteUserClaimsAsync(claim);

            return Ok();
        }

        [HttpGet("{id}/Providers")]
        public async Task<ActionResult<TUserProvidersDto>> GetUserProviders(TUserDtoKey id)
        {
            var userProvidersDto = await _identityService.GetUserProvidersAsync(id.ToString());

            return Ok(userProvidersDto);
        }

        [HttpDelete("Providers")]
        public async Task<IActionResult> DeleteUserProviders([FromBody]TUserProviderDto provider)
        {
            await _identityService.DeleteUserProvidersAsync(provider);

            return Ok();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> PostChangePassword([FromBody]TUserChangePasswordDto password)
        {
            await _identityService.UserChangePasswordAsync(password);

            return Ok();
        }
    }
}