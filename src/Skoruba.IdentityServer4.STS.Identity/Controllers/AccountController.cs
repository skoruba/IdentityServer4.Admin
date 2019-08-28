// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Original file: https://github.com/IdentityServer/IdentityServer4.Samples
// Modified by Jan Škoruba

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using Skoruba.IdentityServer4.STS.Identity.Helpers.ADUtilities;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using Skoruba.IdentityServer4.STS.Identity.ViewModels.Account;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers
{
    [SecurityHeaders]
    [Authorize]
    public class AccountController<TUser, TKey> : Controller
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserResolver<TUser> _userResolver;
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IEmailSender _emailSender;
        private readonly IGenericControllerLocalizer<AccountController<TUser, TKey>> _localizer;
        private readonly LoginConfiguration _loginConfiguration;
        private readonly RegisterConfiguration _registerConfiguration;
        private readonly IADUtilities _ADUtilities;
        private readonly ILogger<AccountController<TUser, TKey>> _logger;

        public AccountController(
            UserResolver<TUser> userResolver,
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IEmailSender emailSender,
            IGenericControllerLocalizer<AccountController<TUser, TKey>> localizer,
            LoginConfiguration loginConfiguration,
            RegisterConfiguration registerConfiguration,
            IADUtilities adUtilities,
            ILogger<AccountController<TUser, TKey>> logger)
        {
            _userResolver = userResolver;
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _emailSender = emailSender;
            _localizer = localizer;
            _loginConfiguration = loginConfiguration;
            _registerConfiguration = registerConfiguration;
            _ADUtilities = adUtilities;
            _logger = logger;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="forceLoginScreen">When true, ignores the AutomaticWindowsLogin setting</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl, bool forceLoginScreen = false)
        {
            if (_loginConfiguration.AutomaticWindowsLogin && !forceLoginScreen && Request.IsFromLocalSubnet())
            {
                return RedirectToAction("ExternalLogin", new { provider = AccountOptions.WindowsAuthenticationSchemeName, returnUrl });
            }
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("ExternalLogin", new { provider = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (await _clientStore.IsPkceClientAsync(context.ClientId))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                    }

                    return Redirect(model.ReturnUrl);
                }

                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }

            if (ModelState.IsValid)
            {
                var user = await _userResolver.GetUserAsync(model.Username);
                if (user != default(TUser))
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberLogin, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));

                        if (context != null)
                        {
                            if (await _clientStore.IsPkceClientAsync(context.ClientId))
                            {
                                // if the client is PKCE then we assume it's native, so this change in how to
                                // return the response is for better UX for the end user.
                                return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                            }

                            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                            return Redirect(model.ReturnUrl);
                        }

                        // request for a local page
                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        if (string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect("~/");
                        }

                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }

                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(LoginWith2fa), new { model.ReturnUrl, RememberMe = model.RememberLogin });
                    }

                    if (result.IsLockedOut)
                    {
                        return View("Lockout");
                    }
                }
                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult WindowsLogin(string returnUrl)
        {
            LoginInputModel vm = new LoginInputModel { ReturnUrl = returnUrl };
            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> WindowsLogin(LoginInputModel vm)
        {
            if (ModelState.IsValid)
            {
                // Username can be in the format domain\username or just the Windows username.
                // In the latter case, the host domain will be used.
                var usernameParts = vm.Username.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                if (usernameParts.Length > 2)
                    ModelState.AddModelError("Username", _localizer["InvalidUsernameFormat"]);
                else
                {
                    var wi = _ADUtilities.LogonWindowsUser(usernameParts.Length > 1 ? usernameParts[1] : usernameParts[0], 
                        vm.Password, 
                        usernameParts.Length > 1 ? usernameParts[0] : null);
                    if (wi != null)
                        return await IssueExternalCookie(vm.ReturnUrl, wi);

                    ModelState.AddModelError(string.Empty, _localizer["WindowsAuthenticationFailed"]);
                }
            }
            
            return View(vm);
        }

        private async Task<IActionResult> IssueExternalCookie(string returnUrl, IIdentity wi)
        {
            var adProperties = _ADUtilities.GetUserInfoFromAD(wi.Name);

            var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
            var name = wi.Name;
            name = name.Substring(name.IndexOf('\\') + 1);

            id.AddClaim(new Claim(JwtClaimTypes.Subject, name));
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, name));
            id.AddClaim(new Claim(JwtClaimTypes.Name, adProperties.DisplayName));
            id.AddClaim(new Claim(JwtClaimTypes.Email, adProperties.Email));

            // we will issue the external cookie and then redirect the
            // user back to the external callback, in essence, treating windows
            // auth the same as any other external authentication mechanism
            var props = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("ExternalLoginCallback"),
                Items =
                    {
                        { "returnUrl", returnUrl },
                        { "LoginProvider", AccountOptions.WindowsAuthenticationSchemeName },
                    }
            };
            await HttpContext.SignInAsync(
                IdentityConstants.ExternalScheme,
                new ClaimsPrincipal(id),
                props);

            return Redirect(props.RedirectUri);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, _localizer["EmailNotFound"]);

                    return View(model);
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(model.Email, _localizer["ResetPasswordTitle"], _localizer["ResetPasswordBody", HtmlEncoder.Default.Encode(callbackUrl)]);


                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }

            AddErrors(result);

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, _localizer["ErrorExternalProvider", remoteError]);

                var vm = await BuildLoginViewModelAsync(returnUrl);
                return View(nameof(Login), vm);
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            var externalResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (externalResult.Succeeded)
            {
                if (externalResult.Properties.Items.ContainsKey("returnUrl"))
                    returnUrl = externalResult.Properties.Items["returnUrl"];

                // We no longer need the external cookie
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                if (info.LoginProvider == AccountOptions.WindowsAuthenticationSchemeName &&
                    _loginConfiguration.SyncUserProfileWithWindows)
                {
                    await SyncUserProfileWithAD(info);
                }
                return RedirectToLocal(returnUrl);
            }

            // If we already collected username and email of the user, auto-provision the user
            var username = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? info.Principal.FindFirstValue(JwtClaimTypes.Subject);
            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? info.Principal.FindFirstValue(JwtClaimTypes.Email);
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
            {
                var user = await ProvideExternalUserAsync(info, username, email);
                if (user == null)
                {
                    var vm = await BuildLoginViewModelAsync(returnUrl);
                    return View(nameof(Login), vm);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }

            // Otherwise ask the user to fill the missing data to create an account
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;

            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email, UserName = username });
        }

        private async Task SyncUserProfileWithAD(ExternalLoginInfo info)
        {
            var adInfo = _ADUtilities.GetUserInfoFromAD(info.ProviderKey);
            var user = await _userResolver.GetUserAsync(info.ProviderKey);

            user.Email = adInfo.Email;
            user.NormalizedEmail = adInfo.Email?.ToUpper();
            user.EmailConfirmed = !string.IsNullOrEmpty(adInfo.Email);
            user.PhoneNumber = adInfo.PhoneNumber;

            var currentClaims = await _userManager.GetClaimsAsync(user);
            await UpdateUserClaim(user, currentClaims, JwtClaimTypes.Email, adInfo.Email);
            await UpdateUserClaim(user, currentClaims, JwtClaimTypes.Picture, adInfo.Photo);
            await UpdateUserClaim(user, currentClaims, JwtClaimTypes.WebSite, adInfo.WebSite);
            await UpdateUserClaim(user, currentClaims, JwtClaimTypes.Address,
                (!string.IsNullOrEmpty(adInfo.Country) || !string.IsNullOrEmpty(adInfo.StreetAddress)) ?
                Newtonsoft.Json.JsonConvert.SerializeObject(new { country = adInfo.Country, street_address = adInfo.StreetAddress }) :
                null);

            if (_loginConfiguration.IncludeWindowsGroups)
            {
                // Remove the groups that the user doesn't belong to anymore.
                // If a policy has been configured for choosing which AD groups should become user claims 
                // (via WindowsGroupsPrefix and WindowsGroupsOURoot settings), only those complying with that policy will be removed
                foreach (var currentGroup in _ADUtilities.FilterADGroups(currentClaims.Where(c => c.Type == JwtClaimTypes.Role).Select(c => c.Value)))
                {
                    if (!adInfo.Groups.Contains(currentGroup))
                    {
                        var removeClaimRes = await _userManager.RemoveClaimAsync(user, currentClaims.First(c => c.Type == JwtClaimTypes.Role && c.Value == currentGroup));
                        if (!removeClaimRes.Succeeded)
                        {
                            _logger.LogError(_localizer["ErrorRemovingRoleClaim", currentGroup, user.UserName]);
                            foreach (var error in removeClaimRes.Errors)
                            {
                                _logger.LogError(error.Description);
                            }
                        }
                    }
                }

                // Add new groups
                foreach (var newGroup in adInfo.Groups)
                {
                    if (currentClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role && c.Value == newGroup) == null)
                    {
                        var addClaimRes = await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, newGroup));
                        if (!addClaimRes.Succeeded)
                        {
                            _logger.LogError(_localizer["ErrorAddingRoleClaim", newGroup, user.UserName]);
                            foreach (var error in addClaimRes.Errors)
                            {
                                _logger.LogError(error.Description);
                            }
                        }
                    }
                }
            }

            var updateUserRes = await _userManager.UpdateAsync(user);
            if (!updateUserRes.Succeeded)
            {
                _logger.LogError(_localizer["ErrorSyncingUserProfile", user.UserName]);
                foreach (var error in updateUserRes.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }
        }

        private async Task<IdentityResult> UpdateUserClaim(TUser user, IList<Claim> currentClaims, string claimType, string claimNewValue)
        {
            IdentityResult res = IdentityResult.Success;
            var currentClaim = currentClaims.FirstOrDefault(c => c.Type == claimType);
            if (currentClaim != null)
            {
                if (string.IsNullOrEmpty(claimNewValue))
                    res = await _userManager.RemoveClaimAsync(user, currentClaim);
                else if (currentClaim.Value != claimNewValue)
                    res = await _userManager.ReplaceClaimAsync(user, currentClaim, new Claim(claimType, claimNewValue));
            }
            else if (!string.IsNullOrEmpty(claimNewValue))
                res = await _userManager.AddClaimAsync(user, new Claim(claimType, claimNewValue));

            if (!res.Succeeded)
            {
                _logger.LogError(_localizer["ErrorUpdatingClaim", claimType, user.UserName, claimNewValue]);
                foreach (var error in res.Errors)
                {
                    _logger.LogError(error.Description);
                }
            }
            return res;
        }        

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                // windows authentication needs special handling
                return await ProcessWindowsLoginAsync(returnUrl);
            }
            else
            {
                // start challenge and roundtrip the return URL and scheme 
                // Request a redirect to the external login provider.
                var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

                return Challenge(properties, provider);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }

            if (ModelState.IsValid)
            {
                var user = await ProvideExternalUserAsync(info, model.UserName, model.Email);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }

            ViewData["LoginProvider"] = info.LoginProvider;
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable2FA"]);
            }

            var model = new LoginWithRecoveryCodeViewModel()
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable2FA"]);
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                return LocalRedirect(string.IsNullOrEmpty(model.ReturnUrl) ? "~/" : model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, _localizer["InvalidRecoveryCode"]);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable2FA"]);
            }

            var model = new LoginWith2faViewModel()
            {
                ReturnUrl = returnUrl,
                RememberMe = rememberMe
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable2FA"]);
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                return LocalRedirect(string.IsNullOrEmpty(model.ReturnUrl) ? "~/" : model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, _localizer["InvalidAuthenticatorCode"]);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            if (!_registerConfiguration.Enabled) return View("RegisterFailure");

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return View(model);

            var user = new TUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(model.Email, _localizer["ConfirmEmailTitle"], _localizer["ConfirmEmailBody", HtmlEncoder.Default.Encode(callbackUrl)]);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToLocal(returnUrl);
            }

            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                    LoginResolutionPolicy = _loginConfiguration.ResolutionPolicy,
                    ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                };
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name, // https://github.com/IdentityServer/IdentityServer4/issues/1607
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                LoginResolutionPolicy = _loginConfiguration.ResolutionPolicy,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            // see if windows auth has already been requested and succeeded
            var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            if (result?.Principal is WindowsPrincipal wp)
            {
                return await IssueExternalCookie(returnUrl, wp.Identity);
            }
            else if (Request.IsFromLocalSubnet())
            {
                // trigger windows auth
                // since windows auth don't support the redirect uri,
                // this URL is re-triggered when we call challenge
                return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
            }
            else
            {
                // if the request comes from another network, the Windows scheme cannot be challenged,
                // so we redirect the user to a Windows login form
                return RedirectToAction("WindowsLogin", new { returnUrl = returnUrl });
            }
        }

        private async Task<TUser> ProvideExternalUserAsync(ExternalLoginInfo info, string username, string email)
        {
            var claims = info.Principal.Claims;

            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            // user's display name
            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            }
            else
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            string thumbnail, phoneNumber, website, country, streetAddress;
            if (info.LoginProvider == AccountOptions.WindowsAuthenticationSchemeName)
            {
                var adInfo = _ADUtilities.GetUserInfoFromAD(info.ProviderKey);
                if(string.IsNullOrEmpty(email))
                    email = adInfo.Email;
                thumbnail = adInfo.Photo;
                phoneNumber = adInfo.PhoneNumber;
                website = adInfo.WebSite;
                country = adInfo.Country;
                streetAddress = adInfo.StreetAddress;

                var roles = adInfo.Groups.Select(x => new Claim(JwtClaimTypes.Role, x));
                filtered.AddRange(roles);
            }
            else
            {
                if (string.IsNullOrEmpty(email))
                    email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
                        claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                thumbnail = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Picture)?.Value;
                phoneNumber = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.PhoneNumber)?.Value;
                website = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.WebSite)?.Value;
                country = null;
                streetAddress = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Address)?.Value;
            }

            if (!string.IsNullOrEmpty(email))
                filtered.Add(new Claim(JwtClaimTypes.Email, email));
            if (!string.IsNullOrEmpty(thumbnail))
                filtered.Add(new Claim(JwtClaimTypes.Picture, thumbnail));
            if (!string.IsNullOrEmpty(website))
                filtered.Add(new Claim(JwtClaimTypes.WebSite, website));
            if (!string.IsNullOrEmpty(country) || !string.IsNullOrEmpty(streetAddress))
                filtered.Add(new Claim(JwtClaimTypes.Address,
                    Newtonsoft.Json.JsonConvert.SerializeObject(new { country = country, street_address = streetAddress })));

            var user = new TUser
            {
                UserName = username ?? info.ProviderKey,
                Email = email,
                NormalizedEmail = email?.ToUpper(),
                EmailConfirmed = info.LoginProvider == AccountOptions.WindowsAuthenticationSchemeName && !string.IsNullOrEmpty(email),
                PhoneNumber = phoneNumber,
            };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                AddErrors(identityResult);
                return null;
            }

            if (filtered.Any())
            {
                identityResult = await _userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded)
                {
                    AddErrors(identityResult);
                    return null;
                }
            }

            identityResult = await _userManager.AddLoginAsync(user, info);
            if (!identityResult.Succeeded)
            {
                AddErrors(identityResult);
                return null;
            }

            return user;
        }
    }
}