using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using Skoruba.IdentityServer4.STS.Identity.ViewModels.Manage;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers
{    
    [Authorize]
    public class ApiController<TUser, TKey> : Controller
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ManageController<TUser, TKey>> _logger;
        private readonly IGenericControllerLocalizer<ManageController<TUser, TKey>> _localizer;
        private readonly UrlEncoder _urlEncoder;

        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        [TempData]
        public string StatusMessage { get; set; }

        public ApiController(UserManager<TUser> userManager, SignInManager<TUser> signInManager, IEmailSender emailSender, ILogger<ManageController<TUser, TKey>> logger, IGenericControllerLocalizer<ManageController<TUser, TKey>> localizer, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _localizer = localizer;
            _urlEncoder = urlEncoder;
        }

        [HttpPost]
        public async Task<IActionResult> updateUser(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(_localizer["UserNotFound", _userManager.GetUserId(User)]);
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException(_localizer["ErrorSettingEmail", user.Id]);
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException(_localizer["ErrorSettingPhone", user.Id]);
                }
            }
            
            await UpdateUserClaimsAsync(model, user);

            StatusMessage = _localizer["ProfileUpdated"];

            return RedirectToAction(nameof(Index));
        }
        
        private async Task<IndexViewModel> BuildManageIndexViewModelAsync(TUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var profile = OpenIdClaimHelpers.ExtractProfileInfo(claims);

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,

                FirstName = profile.FirstName,
                Name = profile.Name,
                MiddleName = profile.MiddleName,
                //Gender = profile.Gender,
                UserType = profile.UserType,
                SignNotifyActive = profile.SignNotifyActive,
                SignPushNotifyActive = profile.SignPushNotifyActive,
                RegionAddrRef = profile.RegionAddrRef,
                UserFromEsia = profile.UserFromEsia,
                SignAutoLocationActive = profile.SignAutoLocationActive,
                AddressFias = profile.AddressFias,
                LoginNameIp = profile.LoginNameIp,
                LoginNameUl = profile.LoginNameUl,
                NameOrg = profile.NameOrg,
                OgrnIp = profile.OgrnIp,
                OgrnUl = profile.OgrnUl,
                Opf = profile.Opf,
                ShowSvetAttributes = profile.ShowSvetAttributes,
                ShowExtendedAttributes = profile.ShowExtendedAttributes,

                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage,
                Website = profile.Website,
                Profile = profile.Profile,
                Country = profile.Country,
                Region = profile.Region,
                PostalCode = profile.PostalCode,
                Locality = profile.Locality,
                StreetAddress = profile.StreetAddress
            };
            return model;
        }

        private async Task UpdateUserClaimsAsync(IndexViewModel model, TUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var oldProfile = OpenIdClaimHelpers.ExtractProfileInfo(claims);
            var newProfile = new OpenIdProfile
            {
                Website = model.Website,
                StreetAddress = model.StreetAddress,
                Locality = model.Locality,
                PostalCode = model.PostalCode,
                Region = model.Region,
                Country = model.Country,
                Profile = model.Profile,

                FirstName = model.FirstName,
                Name = model.Name,
                MiddleName = model.MiddleName,
                //Gender = model.Gender,
                UserType = model.UserType,
                SignNotifyActive = model.SignNotifyActive,
                SignPushNotifyActive = model.SignPushNotifyActive,
                RegionAddrRef = model.RegionAddrRef,
                UserFromEsia = model.UserFromEsia,
                SignAutoLocationActive = model.SignAutoLocationActive,
                AddressFias = model.AddressFias,
                LoginNameIp = model.LoginNameIp,
                LoginNameUl = model.LoginNameUl,
                NameOrg = model.NameOrg,
                OgrnIp = model.OgrnIp,
                OgrnUl = model.OgrnUl,
                Opf = model.Opf,
                ShowSvetAttributes = model.ShowSvetAttributes,
                ShowExtendedAttributes = model.ShowExtendedAttributes,
            };

            var claimsToRemove = OpenIdClaimHelpers.ExtractClaimsToRemove(oldProfile, newProfile);
            var claimsToAdd = OpenIdClaimHelpers.ExtractClaimsToAdd(oldProfile, newProfile);
            var claimsToReplace = OpenIdClaimHelpers.ExtractClaimsToReplace(claims, newProfile);

            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
            await _userManager.AddClaimsAsync(user, claimsToAdd);

            foreach (var pair in claimsToReplace)
            {
                await _userManager.ReplaceClaimAsync(user, pair.Item1, pair.Item2);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;

            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Skoruba.IdentityServer4.STS.Identity"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private void AddError(string description, string title = "")
        {
            ModelState.AddModelError(title, description);
        }
    }
}