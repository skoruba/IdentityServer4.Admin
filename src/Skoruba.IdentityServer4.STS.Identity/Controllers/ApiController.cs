using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Models;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers
{
    [Authorize]
    public class ApiController<TUser, TKey> : Controller
        where TUser : UserIdentity, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ApiController<TUser, TKey>> _logger;
        private readonly IGenericControllerLocalizer<ApiController<TUser, TKey>> _localizer;
        private readonly AccountService<TUser, TKey> _accountService;

        [TempData]
        public string StatusMessage { get; set; }

        public ApiController(UserManager<TUser> userManager, IEmailSender emailSender,
            ILogger<ApiController<TUser, TKey>> logger, IGenericControllerLocalizer<ApiController<TUser, TKey>> localizer,
            AccountService<TUser, TKey> accountService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _localizer = localizer;
            _accountService = accountService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            await _accountService.RegisterAsync(model);
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
                }
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, _localizer["ConfirmEmailTitle"], _localizer["ConfirmEmailBody", HtmlEncoder.Default.Encode(callbackUrl)]);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }
            await _accountService.UpdateUserAsync(user, model);
            return Ok();
        }
    }
}