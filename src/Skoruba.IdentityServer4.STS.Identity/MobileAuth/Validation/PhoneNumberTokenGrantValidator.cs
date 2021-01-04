using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Skoruba.IdentityServer4.STS.Identity.Configuration.Constants.MobileAuthConstants;

namespace Skoruba.IdentityServer4.STS.Identity.MobileAuth.Validation
{
    public class PhoneNumberTokenGrantValidator : IExtensionGrantValidator
    {
        private readonly PhoneNumberTokenProvider<UserIdentity> _phoneNumberTokenProvider;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;
        private readonly IEventService _events;
        private readonly ILogger<PhoneNumberTokenGrantValidator> _logger;

        public PhoneNumberTokenGrantValidator(
            PhoneNumberTokenProvider<UserIdentity> phoneNumberTokenProvider,
            UserManager<UserIdentity> userManager,
            SignInManager<UserIdentity> signInManager,
            IEventService events,
            ILogger<PhoneNumberTokenGrantValidator> logger)
        {
            _phoneNumberTokenProvider = phoneNumberTokenProvider;           
            _userManager = userManager;
            _signInManager = signInManager;
            _events = events;
            _logger = logger;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var raw = context.Request.Raw;
            var acr_values = raw.ToNameValueCollection(OidcConstants.AuthorizeRequest.AcrValues);
            var phoneNumber = raw.Get(TokenRequest.PhoneNumber);
            var verificationToken = raw.Get(TokenRequest.VerificationToken);
            var protectToken = raw.Get(TokenRequest.ProtectToken);

            var device_id = acr_values.Get("device_id");
            var notification_id = acr_values.Get("notification_id");

            var credential = raw.Get(OidcConstants.TokenRequest.GrantType);
            if (credential == null || credential != MobileAuthConstants.GrantType.PhoneNumberToken)
            {
                _logger.LogInformation("Invalid grant_type support", credential);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, $"invalid grant_type {MobileAuthConstants.GrantType.PhoneNumberToken}");
                return;
            }

            if (string.IsNullOrEmpty(device_id))
            {
                _logger.LogInformation("Invalid device_id support", credential);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidTarget, $"invalid device_id ");
                return;
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (user == null)
            {
                _logger.LogInformation("User creation failed: {username}, reason: invalid user", phoneNumber);
                await _events.RaiseAsync(new UserLoginFailureEvent(phoneNumber, "User not found on provided phone_number", false));
                return;
            }


            if (!await _userManager.VerifyUserTokenAsync(user, "Default", TokenPurpose.MobilePasswordAuth, protectToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, $"invalid or missing {TokenRequest.ProtectToken}");
                return;
            }

            var result = await _phoneNumberTokenProvider.ValidateAsync(TokenPurpose.MobilePasswordAuth, verificationToken, _userManager, user);
            if (!result)
            {
                _logger.LogInformation("Authentication failed for token: {token}, reason: invalid token",
                    verificationToken);
                await _events.RaiseAsync(new UserLoginFailureEvent(verificationToken,
                    "invalid token or verification id", false));
                return;
            }



            await _userManager.UpdateSecurityStampAsync(user);

            var claims = await _userManager.GetClaimsAsync(user);

            var claim_device_id = claims.FirstOrDefault(x => x.Type == "device_id");
            if (claim_device_id != null)
                await _userManager.RemoveClaimAsync(user, claim_device_id);

            claim_device_id = new Claim("device_id", device_id);
            var claimresult = await _userManager.AddClaimAsync(user, claim_device_id);

            var claim_notification_id = claims.FirstOrDefault(x => x.Type == "notification_id");
            if (claim_notification_id != null)
                await _userManager.RemoveClaimAsync(user, claim_notification_id);

            claim_notification_id = new Claim("notification_id", notification_id);
            await _userManager.AddClaimAsync(user, claim_notification_id);

            _logger.LogInformation("Credentials validated for username: {phoneNumber}", phoneNumber);
            await _events.RaiseAsync(new UserLoginSuccessEvent(phoneNumber, user.Id.ToString(), phoneNumber, false));
            await _signInManager.SignInAsync(user, true);
            context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.ConfirmationBySms);
        }

        public string GrantType => MobileAuthConstants.GrantType.PhoneNumberToken;
    }
}