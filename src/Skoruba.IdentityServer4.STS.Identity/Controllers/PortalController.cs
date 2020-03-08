using Iserv.IdentityServer4.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using System;
using System.Threading.Tasks;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers
{
    [ApiController]
    [Authorize(AuthorizationConsts.PortalPolicy)]
    [Route("[controller]")]
    public class PortalController<TUser, TKey> : Controller
        where TUser : UserIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly IGenericControllerLocalizer<ApiController<TUser, TKey>> _localizer;
        private readonly IAccountService<TUser, TKey> _accountService;

        public PortalController(IGenericControllerLocalizer<ApiController<TUser, TKey>> localizer,
            IAccountService<TUser, TKey> accountService)
        {
            _localizer = localizer;
            _accountService = accountService;
        }

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        [HttpPost]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var user = await _accountService.FindByIdextAsync(id);
            if (user == null)
                return NotFound(_localizer["UserNotFound", id]);
            await _accountService.UpdateUserFromPortalAsync(user);
            return Ok();
        }
    }
}