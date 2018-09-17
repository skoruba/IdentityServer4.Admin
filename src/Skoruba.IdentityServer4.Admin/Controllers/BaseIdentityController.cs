using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    public class BaseIdentityController<TIdentityDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : BaseController
        where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
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
    {
        private readonly IIdentityService<TIdentityDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> _identityService;
        private readonly IStringLocalizer<IdentityController> _localizer;

        public BaseIdentityController(IIdentityService<TIdentityDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> identityService,
            ILogger<ConfigurationController> logger,
            IStringLocalizer<IdentityController> localizer) : base(logger)
        {
            _identityService = identityService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Roles(int? page, string search)
        {
            ViewBag.Search = search;
            var roles = await _identityService.GetRolesAsync(search, page ?? 1);

            return View(roles);
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Role(TRoleDtoKey id)
        {
            if (EqualityComparer<TRoleDtoKey>.Default.Equals(id, default))
            {
                return View(new TRoleDto());
            }

            var role = await _identityService.GetRoleAsync(new TRoleDto { Id = id });

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Role(TRoleDto role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            if (EqualityComparer<TRoleDtoKey>.Default.Equals(role.Id, default))
            {
                await _identityService.CreateRoleAsync(role);
            }
            else
            {
                await _identityService.UpdateRoleAsync(role);
            }

            SuccessNotification(string.Format(_localizer["SuccessCreateRole"], role.Name), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Roles));
        }

        [HttpGet]
        public async Task<IActionResult> Users(int? page, string search)
        {
            ViewBag.Search = search;
            var usersDto = await _identityService.GetUsersAsync(search, page ?? 1);

            return View(usersDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(TUserDto user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (EqualityComparer<TUserDtoKey>.Default.Equals(user.Id, default))
            {
                await _identityService.CreateUserAsync(user);
            }
            else
            {
                await _identityService.UpdateUserAsync(user);
            }

            SuccessNotification(string.Format(_localizer["SuccessCreateUser"], user.UserName), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public IActionResult UserProfile()
        {
            var newUser = new TUserDto();

            return View("UserProfile", newUser);
        }

        [HttpGet]
        [Route("[controller]/UserProfile/{id}")]
        public async Task<IActionResult> UserProfile(TUserDtoKey id)
        {
            var user = await _identityService.GetUserAsync(id.ToString());
            if (user == null) return NotFound();

            return View("UserProfile", user);
        }

        [HttpGet]
        public async Task<IActionResult> UserRoles(TUserDtoKey id, int? page)
        {
            if (EqualityComparer<TUserDtoKey>.Default.Equals(id, default)) return NotFound();

            var userRoles = await _identityService.BuildUserRolesViewModel(id, page);

            return View(userRoles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserRoles(UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey> role)
        {
            await _identityService.CreateUserRoleAsync(role);
            SuccessNotification(_localizer["SuccessCreateUserRole"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserRoles), new { Id = role.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> UserRolesDelete(TUserDtoKey id, TRoleDtoKey roleId)
        {
            await _identityService.ExistsUserAsync(id.ToString());
            await _identityService.ExistsRoleAsync(roleId.ToString());

            var roles = await _identityService.GetRolesAsync();

            var rolesDto = new UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
            {
                UserId = id,
                RolesList = roles.Select(x => new SelectItem(x.Id.ToString(), x.Name)).ToList(),
                RoleId = roleId
            };

            return View(rolesDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserRolesDelete(UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey> role)
        {
            await _identityService.DeleteUserRoleAsync(role);
            SuccessNotification(_localizer["SuccessDeleteUserRole"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserRoles), new { Id = role.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserClaims(UserClaimsDto<TUserDtoKey> claim)
        {
            if (!ModelState.IsValid)
            {
                return View(claim);
            }

            await _identityService.CreateUserClaimsAsync(claim);
            SuccessNotification(string.Format(_localizer["SuccessCreateUserClaims"], claim.ClaimType, claim.ClaimValue), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserClaims), new { Id = claim.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> UserClaims(TUserDtoKey id, int? page)
        {
            if (EqualityComparer<TUserDtoKey>.Default.Equals(id, default)) return NotFound();

            var claims = await _identityService.GetUserClaimsAsync(id.ToString(), page ?? 1);
            claims.UserId = id;

            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> UserClaimsDelete(TUserDtoKey id, int claimId)
        {            
            if (EqualityComparer<TUserDtoKey>.Default.Equals(id, default)
            || EqualityComparer<int>.Default.Equals(claimId, default)) return NotFound();

            var claim = await _identityService.GetUserClaimAsync(id.ToString(), claimId);
            if (claim == null) return NotFound();

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserClaimsDelete(UserClaimsDto<TUserDtoKey> claim)
        {
            await _identityService.DeleteUserClaimsAsync(claim);
            SuccessNotification(_localizer["SuccessDeleteUserClaims"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserClaims), new { Id = claim.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> UserProviders(TUserDtoKey id)
        {
            if (EqualityComparer<TUserDtoKey>.Default.Equals(id, default)) return NotFound();
            
            var providers = await _identityService.GetUserProvidersAsync(id.ToString());

            return View(providers);
        }

        [HttpGet]
        public async Task<IActionResult> UserProvidersDelete(TUserDtoKey id, string providerKey)
        {
            if (EqualityComparer<TUserDtoKey>.Default.Equals(id, default) || string.IsNullOrEmpty(providerKey)) return NotFound();

            var provider = await _identityService.GetUserProviderAsync(id.ToString(), providerKey);
            if (provider == null) return NotFound();

            return View(provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProvidersDelete(UserProviderDto<TUserDtoKey> provider)
        {
            await _identityService.DeleteUserProvidersAsync(provider);
            SuccessNotification(_localizer["SuccessDeleteUserProviders"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(UserProviders), new { Id = provider.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> UserChangePassword(TUserDtoKey id)
        {
            if (EqualityComparer<TUserDtoKey>.Default.Equals(id, default)) return NotFound();

            var user = await _identityService.GetUserAsync(id.ToString());
            var userDto = new UserChangePasswordDto<TUserDtoKey> { UserId = id, UserName = user.UserName };

            return View(userDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserChangePassword(UserChangePasswordDto<TUserDtoKey> userPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(userPassword);
            }

            var identityResult = await _identityService.UserChangePasswordAsync(userPassword);

            if (!identityResult.Errors.Any())
            {
                SuccessNotification(_localizer["SuccessUserChangePassword"], _localizer["SuccessTitle"]);

                return RedirectToAction("UserProfile", new { Id = userPassword.UserId });
            }

            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(userPassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleClaims(RoleClaimsDto<TRoleDtoKey> claim)
        {
            if (!ModelState.IsValid)
            {
                return View(claim);
            }

            await _identityService.CreateRoleClaimsAsync(claim);
            SuccessNotification(string.Format(_localizer["SuccessCreateRoleClaims"], claim.ClaimType, claim.ClaimValue), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(RoleClaims), new { Id = claim.RoleId });
        }

        [HttpGet]
        public async Task<IActionResult> RoleClaims(TRoleDtoKey id, int? page)
        {
            if (EqualityComparer<TRoleDtoKey>.Default.Equals(id, default)) return NotFound();

            var claims = await _identityService.GetRoleClaimsAsync(id.ToString(), page ?? 1);
            claims.RoleId = id;

            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> RoleClaimsDelete(TRoleDtoKey id, int claimId)
        {
            if (EqualityComparer<TRoleDtoKey>.Default.Equals(id, default) ||
                EqualityComparer<int>.Default.Equals(claimId, default)) return NotFound();

            var claim = await _identityService.GetRoleClaimAsync(id.ToString(), claimId);

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleClaimsDelete(RoleClaimsDto<TRoleDtoKey> claim)
        {
            await _identityService.DeleteRoleClaimsAsync(claim);
            SuccessNotification(_localizer["SuccessDeleteRoleClaims"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(RoleClaims), new { Id = claim.RoleId });
        }

        [HttpGet]
        public async Task<IActionResult> RoleDelete(TRoleDtoKey id)
        {
            if (EqualityComparer<TRoleDtoKey>.Default.Equals(id, default)) return NotFound();

            var roleDto = await _identityService.GetRoleAsync(new TRoleDto { Id = id });
            if (roleDto == null) return NotFound();

            return View(roleDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleDelete(TRoleDto role)
        {
            await _identityService.DeleteRoleAsync(role);
            SuccessNotification(_localizer["SuccessDeleteRole"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDelete(TUserDto user)
        {
            await _identityService.DeleteUserAsync(user.Id.ToString(), user);
            SuccessNotification(_localizer["SuccessDeleteUser"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public async Task<IActionResult> UserDelete(TUserDtoKey id)
        {
            if (EqualityComparer<TUserDtoKey>.Default.Equals(id, default)) return NotFound();

            var user = await _identityService.GetUserAsync(id.ToString());
            if (user == null) return NotFound();

            return View(user);
        }
    }
}