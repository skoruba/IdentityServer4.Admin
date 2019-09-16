using System;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.Api.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Api.Dtos.Roles;
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
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizationConsts.AdministrationPolicy)]
    public class RolesController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> : ControllerBase
        where TUserDto : UserDto<TKey>, new()
        where TRoleDto : RoleDto<TKey>, new()
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
        where TUserClaimsDto : UserClaimsDto<TKey>, new()
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TKey>, new()
    {
        private readonly IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> _identityService;
        private readonly IGenericControllerLocalizer<UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>> _localizer;

        private readonly IMapper _mapper;

        public RolesController(IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> identityService,
            IGenericControllerLocalizer<UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>> localizer, IMapper mapper)
        {
            _identityService = identityService;
            _localizer = localizer;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TRoleDto>> Get(TKey id)
        {
            var role = await _identityService.GetRoleAsync(id);

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
            await _identityService.GetRoleAsync(role.Id);
            await _identityService.UpdateRoleAsync(role);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(TKey id)
        {
            var roleDto = new TRoleDto { Id = id };

            await _identityService.GetRoleAsync(id);
            await _identityService.DeleteRoleAsync(roleDto);

            return Ok();
        }

        [HttpGet("{id}/Users")]
        public async Task<ActionResult<TRolesDto>> GetRoleUsers(TKey id, string searchText, int page = 1, int pageSize = 10)
        {
            var usersDto = await _identityService.GetRoleUsersAsync(id, searchText, page, pageSize);

            return Ok(usersDto);
        }

        [HttpGet("{id}/Claims")]
        public async Task<ActionResult<RoleClaimsApiDto<TKey>>> GetRoleClaims(TKey id, int page = 1, int pageSize = 10)
        {
            var roleClaimsDto = await _identityService.GetRoleClaimsAsync(id, page, pageSize);
            var roleClaimsApiDto = _mapper.Map<RoleClaimsApiDto<TKey>>(roleClaimsDto);

            return Ok(roleClaimsApiDto);
        }

        [HttpPost("Claims")]
        public async Task<IActionResult> PostRoleClaims([FromBody]RoleClaimApiDto<TKey> roleClaims)
        {
            var roleClaimsDto = _mapper.Map<TRoleClaimsDto>(roleClaims);
            await _identityService.CreateRoleClaimsAsync(roleClaimsDto);

            return Ok();
        }

        [HttpDelete("{id}/Claims")]
        public async Task<IActionResult> DeleteRoleClaims(TKey id, int claimId)
        {
            var roleDto = new TRoleClaimsDto { ClaimId = claimId, RoleId = id };

            await _identityService.GetRoleClaimAsync(roleDto.RoleId, roleDto.ClaimId);
            await _identityService.DeleteRoleClaimsAsync(roleDto);

            return Ok();
        }
    }
}