using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using Skoruba.IdentityServer4.STS.Identity.ViewModels.Account;

namespace Skoruba.IdentityServer4.STS.Identity.api.v1.Account
{
	[Route("api/v1/[controller]")]
	public class AccountController<TUser, TKey> : Controller where TUser : IdentityUser<TKey>, new() where TKey : IEquatable<TKey>
	{
		protected UserManager<TUser> UserManager { get; }
		protected SignInManager<TUser> SignInManager { get; }
		protected IEventService Events { get; }
		protected IEmailSender EmailSender { get; }
		protected LoginConfiguration LoginConfiguration { get; }
		protected RegisterConfiguration RegisterConfiguration { get; }
		protected IGenericControllerLocalizer<AccountController<TUser, TKey>> Localizer { get; }

		public AccountController(
			UserManager<TUser> userManager,
			SignInManager<TUser> signInManager,
			IEventService events,
			IEmailSender emailSender,
			RegisterConfiguration registerConfiguration,
			LoginConfiguration loginConfiguration,
			IGenericControllerLocalizer<AccountController<TUser, TKey>> localizer)
		{
			UserManager = userManager;
			SignInManager = signInManager;
			Events = events;
			EmailSender = emailSender;
			LoginConfiguration = loginConfiguration;
			RegisterConfiguration = registerConfiguration;
			Localizer = localizer;
		}

		// GET api/v1/<controller>/<action>
		[HttpGet("[action]")]
		public async Task<IStatusCodeActionResult> Logout()
		{
			if (User?.Identity.IsAuthenticated == true)
			{
				// delete local authentication cookie
				await SignInManager.SignOutAsync();

				// raise the logout event
				await Events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
			}

			return Ok(new { Success = true, Message = "signed out" });
		}

		// POST api/v1/<controller>/<action>
		[HttpPost("[action]")]
		public async Task<IStatusCodeActionResult> Login([FromBody] LoginViewModel login)
		{
			// TODO: Issue a JWT instead of default (cookie?)
			// Note: This is probably not needed. It's useful if you're doing cookies, but if we want a JWT, then we should be using /connect/token
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByNameAsync(login.User) ?? await UserManager.FindByEmailAsync(login.User);

				if (user != default(TUser))
				{
					var result = await SignInManager.PasswordSignInAsync(user.UserName, login.Password, login.RememberLogin, lockoutOnFailure: true);
					if (result.Succeeded)
					{
						await Events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));
						return Ok(new { Success = true, TwoFactorRequired = false, Message = "logged in" });
					}

					if (result.RequiresTwoFactor)
					{
						return Ok(new { Success = true, TwoFactorRequired = true, Message = "two factor required" });
					}

					if (result.IsLockedOut)
					{
						return BadRequest(new { Success = false, TwoFactorRequired = false, Message = "accout locked" });
					}
				}
			}

			await Events.RaiseAsync(new UserLoginFailureEvent(login.User, "invalid credentials"));
			return NotFound(new { Success = false, TwoFactorRequired = false, Message = AccountOptions.InvalidCredentialsErrorMessage });
		}

		// POST api/v1/<controller>/<action>
		[HttpPost("[action]")]
		public async Task<IStatusCodeActionResult> TwoFactorConfirmation([FromBody]LoginWith2faViewModel model)
		{
			// Ensure the user has gone through the username & password screen first
			var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				throw new InvalidOperationException(Localizer["Unable2FA"]);
			}

			var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

			var result = await SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberMachine);

			if (result.Succeeded)
			{
				return Ok(new { Message = "success" });
			}

			if (result.IsLockedOut)
			{
				return BadRequest(new { Message = "account is locked out" });
			}

			return BadRequest(new { Message = Localizer["InvalidAuthenticatorCode"] });
		}

		// POST api/v1/<controller>/<action>
		[HttpPost("[action]")]
		public async Task<IStatusCodeActionResult> CheckUserAvailable([FromBody] CheckUserAvailableModel login)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var user = await UserManager.FindByNameAsync(login.Username) ?? await UserManager.FindByEmailAsync(login.Email);
			if (user == default)
			{
				return Ok(new { Success = true, Message = "username / email is available" });
			}

			return Ok(new { Success = false, Message = "Username or email has already been registered" });
		}

		// POST api/v1/<controller>/<action>
		[HttpPost("[action]")]
		public async Task<IStatusCodeActionResult> Register([FromBody] RegisterViewModel model)
		{
			if (!ModelState.IsValid) 
				return BadRequest(new { Succeeded = false, Errors = ModelState.Select(n => $"{n.Key}: {string.Join(',', n.Value.Errors.Select(m => m.ErrorMessage))}") });

			var user = new TUser
			{
				UserName = model.UserName,
				Email = model.Email
			};

			var result = await UserManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
				var callbackUrl = $"{RegisterConfiguration.ApiConfirmationUrl}?userid={user.Id}&code={HttpUtility.UrlEncode(code)}";

				await EmailSender.SendEmailAsync(model.Email, Localizer["ConfirmEmailTitle"], Localizer["ConfirmEmailBody", callbackUrl]);
				await SignInManager.SignInAsync(user, isPersistent: false);

				return Ok(new { Succeeded = true });
			}

			return BadRequest(new { Succeeded = false, Errors = result.Errors.Select(n => $"{n.Code}: {n.Description}") });
		}

		// POST api/v1/<controller>/<action>
		[HttpPost("[action]")]
		public async Task<IStatusCodeActionResult> ConfirmEmail([FromBody]ConfirmEmailModel model)
		{
			if (string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.Code))
				return BadRequest();

			var user = await UserManager.FindByIdAsync(model.UserId);
			if (user == null)
				return BadRequest();

			var result = await UserManager.ConfirmEmailAsync(user, model.Code);
			if (result.Succeeded)
				return Ok();
			else
				return BadRequest();
		}

		// POST api/v1/<controller>/<action>
		[HttpPost("[action]")]
		public async Task<IStatusCodeActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { Succeeded = false, Errors = ModelState.Select(n => $"{n.Key}: {string.Join(',', n.Value.Errors.Select(m => m.ErrorMessage))}") });

			var user = await UserManager.FindByEmailAsync(model.Email);

			if (user == null || !await UserManager.IsEmailConfirmedAsync(user))
				return BadRequest(new { Succeeded = false, Errors = new [] { Localizer["EmailNotFound"].Value } });

			var code = await UserManager.GeneratePasswordResetTokenAsync(user);
			var callbackUrl = $"{LoginConfiguration.ApiForgotPasswordCallbackUrl}?email={user.Email}&code={HttpUtility.UrlEncode(code)}";

			await EmailSender.SendEmailAsync(model.Email, Localizer["ResetPasswordTitle"], Localizer["ResetPasswordBody", callbackUrl]);

			return Ok(new { Succeeded = true });
		}

		// POST api/v1/<controller>/<action>
		[HttpPost("[action]")]
		public async Task<IStatusCodeActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { Succeeded = false, Errors = ModelState.Select(n => $"{n.Key}: {string.Join(',', n.Value.Errors.Select(m => m.ErrorMessage))}") });

			var user = await UserManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return BadRequest();
			}

			var result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded)
			{
				return Ok(new { Succeeded = true });
			}

			return BadRequest(result);
		}

	}
}
