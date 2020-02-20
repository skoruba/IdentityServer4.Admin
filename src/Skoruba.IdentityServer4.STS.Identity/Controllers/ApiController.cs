using Iserv.IdentityServer4.BusinessLogic.Extension;
using Iserv.IdentityServer4.BusinessLogic.Models;
using Iserv.IdentityServer4.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iserv.IdentityServer4.BusinessLogic.Helpers;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
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
        private readonly IGenericControllerLocalizer<ApiController<TUser, TKey>> _localizer;
        private readonly IAccountService<TUser, TKey> _accountService;

        public ApiController(UserManager<TUser> userManager,
            IGenericControllerLocalizer<ApiController<TUser, TKey>> localizer,
            IAccountService<TUser, TKey> accountService)
        {
            _userManager = userManager;
            _localizer = localizer;
            _accountService = accountService;
        }
        
        /// <summary>
        /// Получение данных пользователя
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(string), 500)]
        [Route("userInfo")]
        public async Task<IActionResult> UserInfoAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _accountService.GetExtraFieldsAsync(user);
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
        public async Task<IActionResult> RequestCheckPhoneAsync([FromBody] string phone)
        {
            await _accountService.RequestCheckPhoneAsync(phone, false);
            return Ok();
        }

        /// <summary>
        /// Проверка кода верификации номера телефона по смс
        /// </summary>
        /// <param name="model">Модель проверки номера телефона</param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 500)]
        [Route("validSmsCode")]
        public IActionResult ValidSmsCodeAsync([FromBody] ValidSmsCodeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            _accountService.ValidSmsCode(model.PhoneNumber, model.SmsCode);
            return Ok();
        }

        /// <summary>
        /// Проверка кода верификации номера телефона по смс
        /// </summary>
        /// <param name="model">Данные регистрации нового пользователя</param>
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            await _accountService.RegisterAsync(model);
            return Ok();
        }

        /// <summary>
        /// Изменение пользователя
        /// </summary>
        /// <param name="model">Обновляемые данные пользователя</param>
        [HttpPut]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUserAsync([FromForm] UpdateUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            model.Id = Guid.Parse(user.Id.ToString());
            if (Request.Form.TryGetValue("values", out var values))
            {
                model.Values = JsonConvert.DeserializeObject<Dictionary<string, object>>(values);
            }
            model.Files = Request.Form.Files.Select(f => f.ConvertToFileModel());
            await _accountService.UpdateUserAsync(model);
            return Ok();
        }
        
        /// <summary>
        /// Изменение email пользователя
        /// </summary>
        /// <param name="model">Модель изменения email пользователя</param>
        [HttpPut]
        [Route("changeEmail")]
        public async Task<IActionResult> ChangeEmailAsync([FromForm] ChangeEmailModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            await _accountService.ChangeEmailAsync(user, model.Email, model.EmailCode);
            return Ok();
        }
        
        /// <summary>
        /// Изменение номера телбефона пользователя
        /// </summary>
        /// <param name="model">Модель изменения номера телефона пользователя</param>
        [HttpPut]
        [Route("changePhone")]
        public async Task<IActionResult> ChangePhoneAsync([FromForm] ChangePhoneModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            await _accountService.ChangePhoneAsync(user, model.PhoneNumber, model.SmsCode);
            return Ok();
        }
        
        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="password">Новый пароль пользователя</param>
        [HttpPut]
        [Route("updatePassword")]
        public async Task<IActionResult> UpdatePasswordAsync([FromBody] string password)
        {
            await _accountService.UpdatePasswordAsync(UserClaimsHelpers.GetIdext(User.Identity), password);
            return Ok();
        }
        
        /// <summary>
        /// Востановление пароля пользователя через email
        /// </summary>
        /// <param name="email">Email востановления пароля пользователя</param>
        [HttpPost]
        [Route("restorePasswordByEmail")]
        public async Task<IActionResult> RestorePasswordByEmailAsync([FromBody] string email)
        {
            await _accountService.RestorePasswordByEmailAsync(email);
            return Ok();
        }
        
        /// <summary>
        /// Востановление пароля пользователя через телефон
        /// </summary>
        /// <param name="phoneNumber">Номер телефона востановления пароля пользователя</param>
        [HttpPost]
        [Route("repairPasswordBySms")]
        public async Task<IActionResult> RepairPasswordBySmsAsync([FromBody] string phoneNumber)
        {
            await _accountService.RepairPasswordBySmsAsync(phoneNumber);
            return Ok();
        }
        
        /// <summary>
        /// Проверка смс кода для востановление пароля пользователя через телефон
        /// </summary>
        /// <param name="model">Модель проверки номера телефона</param>
        [HttpPost]
        [Route("repairPasswordBySms")]
        public IActionResult ValidSmsCodeChangePassword([FromBody] ValidSmsCodeModel model)
        {
            _accountService.ValidSmsCodeChangePassword(model.PhoneNumber, model.SmsCode);
            return Ok();
        }
        
        /// <summary>
        /// Изменение пароля через смс код востановления пароля пользователя через телефон
        /// </summary>
        /// <param name="model">Модель изменения пароля через смс код востановления пароля пользователя через телефон</param>
        [HttpPost]
        [Route("repairPasswordBySms")]
        public IActionResult ChangePasswordBySmsAsync([FromBody] ChangePasswordBySmsCodeModel model)
        {
            _accountService.ChangePasswordBySmsAsync(model.PhoneNumber, model.SmsCode, model.Password);
            return Ok();
        }
    }
}