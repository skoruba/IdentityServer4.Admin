using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorubaIdentityServer4Admin.Admin.Api.Configuration.Constants;
using SkorubaIdentityServer4Admin.Admin.Api.Dtos.Roles;
using SkorubaIdentityServer4Admin.Admin.Api.Dtos.Users;
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
        where TUserClaimsDto : UserClaimsDto<TUserDtoKey>, new()
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

        private readonly IMapper _mapper;
        private readonly IApiErrorResources _errorResources;

        public UsersController(IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> identityService,
            IGenericControllerLocalizer<UsersController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>> localizer, IMapper mapper, IApiErrorResources errorResources)
        {
            _identityService = identityService;
            _localizer = localizer;
            _mapper = mapper;
            _errorResources = errorResources;
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
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TUserDto>> Post([FromBody]TUserDto user)
        {
            if (!EqualityComparer<TUserDtoKey>.Default.Equals(user.Id, default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var (identityResult, userId) = await _identityService.CreateUserAsync(user);
            var createdUser = await _identityService.GetUserAsync(userId.ToString());

            return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]TUserDto user)
        {
            await _identityService.GetUserAsync(user.Id.ToString());
            await _identityService.UpdateUserAsync(user);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(TUserDtoKey id)
        {
            var currentUserId = User.GetSubjectId();
            if (id.ToString() == currentUserId)
                return StatusCode((int)System.Net.HttpStatusCode.Forbidden);

            var user = new TUserDto { Id = id };

            await _identityService.GetUserAsync(user.Id.ToString());
            await _identityService.DeleteUserAsync(user.Id.ToString(), user);

            return Ok();
        }

        [HttpGet("{id}/Roles")]
        public async Task<ActionResult<UserRolesApiDto<TRoleDto>>> GetUserRoles(TRoleDtoKey id, int page = 1, int pageSize = 10)
        {
            var userRoles = await _identityService.GetUserRolesAsync(id.ToString(), page, pageSize);
            var userRolesApiDto = _mapper.Map<UserRolesApiDto<TRoleDto>>(userRoles);

            return Ok(userRolesApiDto);
        }

        [HttpPost("Roles")]
        public async Task<IActionResult> PostUserRoles([FromBody]UserRoleApiDto<TUserDtoKey, TRoleDtoKey> role)
        {
            var userRolesDto = _mapper.Map<TUserRolesDto>(role);

            await _identityService.CreateUserRoleAsync(userRolesDto);

            return Ok();
        }

        [HttpDelete("Roles")]
        public async Task<IActionResult> DeleteUserRoles([FromBody]UserRoleApiDto<TUserDtoKey, TRoleDtoKey> role)
        {
            var userRolesDto = _mapper.Map<TUserRolesDto>(role);

            await _identityService.GetUserAsync(userRolesDto.UserId.ToString());
            await _identityService.GetRoleAsync(userRolesDto.RoleId.ToString());

            await _identityService.DeleteUserRoleAsync(userRolesDto);

            return Ok();
        }

        [HttpGet("{id}/Claims")]
        public async Task<ActionResult<UserClaimsApiDto<TUserDtoKey>>> GetUserClaims(TUserDtoKey id, int page = 1, int pageSize = 10)
        {
            var claims = await _identityService.GetUserClaimsAsync(id.ToString(), page, pageSize);

            var userClaimsApiDto = _mapper.Map<UserClaimsApiDto<TUserDtoKey>>(claims);

            return Ok(userClaimsApiDto);
        }

        [HttpPost("Claims")]
        public async Task<IActionResult> PostUserClaims([FromBody]UserClaimApiDto<TUserDtoKey> claim)
        {
            var userClaimDto = _mapper.Map<TUserClaimsDto>(claim);

            if (!userClaimDto.ClaimId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            await _identityService.CreateUserClaimsAsync(userClaimDto);

            return Ok();
        }

        [HttpDelete("{id}/Claims")]
        public async Task<IActionResult> DeleteUserClaims([FromRoute]TUserDtoKey id, int claimId)
        {
            var userClaimsDto = new TUserClaimsDto
            {
                ClaimId = claimId,
                UserId = id
            };

            await _identityService.GetUserClaimAsync(id.ToString(), claimId);
            await _identityService.DeleteUserClaimsAsync(userClaimsDto);

            return Ok();
        }

        [HttpGet("{id}/Providers")]
        public async Task<ActionResult<UserProvidersApiDto<TUserDtoKey>>> GetUserProviders(TUserDtoKey id)
        {
            var userProvidersDto = await _identityService.GetUserProvidersAsync(id.ToString());
            var userProvidersApiDto = _mapper.Map<UserProvidersApiDto<TUserDtoKey>>(userProvidersDto);

            return Ok(userProvidersApiDto);
        }

        [HttpDelete("Providers")]
        public async Task<IActionResult> DeleteUserProviders([FromBody]UserProviderDeleteApiDto<TUserDtoKey> provider)
        {
            var providerDto = _mapper.Map<TUserProviderDto>(provider);

            await _identityService.GetUserProviderAsync(providerDto.UserId.ToString(), providerDto.ProviderKey);
            await _identityService.DeleteUserProvidersAsync(providerDto);

            return Ok();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> PostChangePassword([FromBody]UserChangePasswordApiDto<TUserDtoKey> password)
        {
            var userChangePasswordDto = _mapper.Map<TUserChangePasswordDto>(password);

            await _identityService.UserChangePasswordAsync(userChangePasswordDto);

            return Ok();
        }

		[HttpGet("{id}/RoleClaims")]
		public async Task<ActionResult<RoleClaimsApiDto<TRoleDtoKey>>> GetRoleClaims(TUserDtoKey id, string claimSearchText, int page = 1, int pageSize = 10)
		{
			var roleClaimsDto = await _identityService.GetUserRoleClaimsAsync(id.ToString(), claimSearchText, page, pageSize);
			var roleClaimsApiDto = _mapper.Map<RoleClaimsApiDto<TRoleDtoKey>>(roleClaimsDto);

			return Ok(roleClaimsApiDto);
		}
	}
}





