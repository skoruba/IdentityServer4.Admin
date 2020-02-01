using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Helpers.Localization;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class ExportController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> : BaseController
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
        private readonly IExportService _exportService;
        private readonly IExportIdentityService _exportIdentityService;
        private readonly IGenericControllerLocalizer<IdentityController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
           TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
           TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>> _localizer;

        public ExportController(IExportService exportService,
            IExportIdentityService exportIdentityService,
            IGenericControllerLocalizer<IdentityController<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>> localizer,
            ILogger<ConfigurationController> logger) : base(logger)
        {
            _exportIdentityService = exportIdentityService;
            _exportService = exportService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> ExportConfig()
        {
            var stream = new MemoryStream(await _exportService.GetExportBytesConfigAsync());
            return new FileStreamResult(stream, "application/....")
            {
                FileDownloadName = $"config_{DateTime.Now.ToString("d")}.json"
            };
        }

        [HttpPost]
        public async Task<IActionResult> ImportConfig(IFormFile file)
        {
            if (file == null)
            {
                return new EmptyResult();
            }
            if (Path.GetExtension(file.FileName) != ".json")
            {
                return BadRequest("Invalid file extension");
            }
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                await _exportService.ImportConfigAsync(await reader.ReadToEndAsync());
            }
            SuccessNotification("Конфигурация обновлена", _localizer["SuccessTitle"]);
            return RedirectToAction("Clients", "Configuration");
        }

        [HttpPost]
        public async Task<IActionResult> ImportUsers(IFormFile file)
        {
            if (file == null)
            {
                return new EmptyResult();
            }
            if (Path.GetExtension(file.FileName) != ".json")
            {
                return BadRequest("Invalid file extension");
            }
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                await _exportIdentityService.ImportUsersAsync(await reader.ReadToEndAsync());
            }
            SuccessNotification("Пользователи добавлены", _localizer["SuccessTitle"]);
            return RedirectToAction("Users", "Identity");
        }
    }
}
