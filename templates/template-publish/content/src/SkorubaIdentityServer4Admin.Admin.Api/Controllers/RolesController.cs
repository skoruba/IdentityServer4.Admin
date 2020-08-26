using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorubaIdentityServer4Admin.Admin.Api.Configuration.Constants;
using SkorubaIdentityServer4Admin.Admin.Api.Dtos.Roles;
using SkorubaIdentityServer4Admin.Admin.Api.ExceptionHandling;
using SkorubaIdentityServer4Admin.Admin.Api.Helpers.Localization;
using SkorubaIdentityServer4Admin.Admin.Api.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;

namespace SkorubaIdentityServer4Admin.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class RolesController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto> : ControllerBase
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
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>, new()
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TKey>, new()
        where TUserClaimDto : UserClaimDto<TKey>
    {
        private readonly IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto> _identityService;
        private readonly IGenericControllerLocalizer<UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto>> _localizer;

        private readonly IMapper _mapper;
        private readonly IApiErrorResources _errorResources;

        public RolesController(IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto> identityService,
            IGenericControllerLocalizer<UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto>> localizer, IMapper mapper, IApiErrorResources errorResources)
        {
            _identityService = identityService;
            _localizer = localizer;
            _mapper = mapper;
            _errorResources = errorResources;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TRoleDto>> Get(TKey id)
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
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TRoleDto>> Post([FromBody]TRoleDto role)
        {
            if (!EqualityComparer<TKey>.Default.Equals(role.Id, default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }
 
            var (identityResult, roleId) = await _identityService.CreateRoleAsync(role);
            var createdRole = await _identityService.GetRoleAsync(roleId.ToString());

            return CreatedAtAction(nameof(Get), new { id = createdRole.Id }, createdRole);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]TRoleDto role)
        {
            await _identityService.GetRoleAsync(role.Id.ToString());
            await _identityService.UpdateRoleAsync(role);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(TKey id)
        {
            var roleDto = new TRoleDto { Id = id };

            await _identityService.GetRoleAsync(id.ToString());
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
        public async Task<ActionResult<RoleClaimsApiDto<TKey>>> GetRoleClaims(string id, int page = 1, int pageSize = 10)
        {
            var roleClaimsDto = await _identityService.GetRoleClaimsAsync(id, page, pageSize);
            var roleClaimsApiDto = _mapper.Map<RoleClaimsApiDto<TKey>>(roleClaimsDto);

            return Ok(roleClaimsApiDto);
        }

        [HttpPost("Claims")]
        public async Task<IActionResult> PostRoleClaims([FromBody]RoleClaimApiDto<TKey> roleClaims)
        {
            var roleClaimsDto = _mapper.Map<TRoleClaimsDto>(roleClaims);

            if (!roleClaimsDto.ClaimId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            await _identityService.CreateRoleClaimsAsync(roleClaimsDto);

            return Ok();
        }

        [HttpPut("Claims")]
        public async Task<IActionResult> PutRoleClaims([FromBody]RoleClaimApiDto<TKey> roleClaims)
        {
            var roleClaimsDto = _mapper.Map<TRoleClaimsDto>(roleClaims);

            if (!roleClaimsDto.ClaimId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            await _identityService.UpdateRoleClaimsAsync(roleClaimsDto);

            return Ok();
        }

        [HttpDelete("{id}/Claims")]
        public async Task<IActionResult> DeleteRoleClaims(TKey id, int claimId)
        {
            var roleDto = new TRoleClaimsDto { ClaimId = claimId, RoleId = id };

            await _identityService.GetRoleClaimAsync(roleDto.RoleId.ToString(), roleDto.ClaimId);
            await _identityService.DeleteRoleClaimAsync(roleDto);

            return Ok();
        }
    }
}





