using DataAnnotationsExtensions;
using Iserv.IdentityServer4.BusinessLogic.Extension;
using Iserv.IdentityServer4.BusinessLogic.Models;
using Iserv.IdentityServer4.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers
{
    [SecurityHeaders]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    [Route("[controller]")]
    public class ApiController<TUser, TKey> : Controller
        where TUser : UserIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ApiController<TUser, TKey>> _logger;
        private readonly IGenericControllerLocalizer<ApiController<TUser, TKey>> _localizer;
        private readonly IAccountService<TUser, TKey> _accountService;

        [TempData]
        public string StatusMessage { get; set; }

        public ApiController(UserManager<TUser> userManager, IEmailSender emailSender,
            ILogger<ApiController<TUser, TKey>> logger,
            IGenericControllerLocalizer<ApiController<TUser, TKey>> localizer,
            IAccountService<TUser, TKey> accountService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _localizer = localizer;
            _accountService = accountService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), 500)]
        [Route("userinfo")]
        public async Task<IActionResult> Userinfo()
        {
            var user = await _userManager.GetUserAsync(User);
            var claims = (await _userManager.GetClaimsAsync(user)).ToArray();
            var result = claims.ToDictionary(c => c.Type, c => c.Value);
            result.Add("id", user.Id.ToString());
            result.Add("idext", user.Idext.ToString());
            result.Add("email", user.Email);
            result.Add("emailConfirmed", user.EmailConfirmed.ToString().ToLower());
            result.Add("phone", user.PhoneNumber);
            result.Add("phoneConfirmed", user.PhoneNumberConfirmed.ToString().ToLower());
            return Ok(result);
        }

        /// <summary>
        /// Запрос на проверку подленности номера телефона пользователю по смс
        /// </summary>
        /// <param name="phone">Проверяемый номер телефона</param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 500)]
        [Route("requestCheckPhone")]
        public async Task<IActionResult> RequestCheckPhoneAsync(string phone)
        {
            await _accountService.RequestCheckPhoneAsync(phone);
            return Ok();
        }

        /// <summary>
        /// Проверка кода верификации номера телефона по смс
        /// </summary>
        /// <param name="phone">Проверяемый номер телефона</param>
        /// <param name="code">Код смс проверяемого номера телефона</param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 500)]
        [Route("validSMSCodeCheckPhone")]
        public IActionResult ValidSMSCodeCheckPhone(string phone, string code)
        {
            _accountService.ValidSMSCodeCheckPhone(phone, code);
            return Ok();
        }

        /// <summary>
        /// Проверка кода верификации номера телефона по смс
        /// </summary>
        /// <param name="phone">Проверяемый номер телефона</param>
        /// <param name="code">Код смс проверяемого номера телефона</param>
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegistrUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            await _accountService.RegisterAsync(model);
            return Ok();
        }

        [HttpPut]
        [Route("changeEmail")]
        public async Task<IActionResult> ChangeEmailAsync([FromBody] string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            var callbackUrlFormat = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = "{0}" }, HttpContext.Request.Scheme);
            await _accountService.ChangeEmailAsync(user, email);
            return Ok();
        }

        [HttpPut]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] Dictionary<string, object> values)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            await _accountService.UpdateUserAsync(user, values, Request.Form.Files.Select(f => f.ConvertToFileModel()));
            return Ok();
        }
    }
}