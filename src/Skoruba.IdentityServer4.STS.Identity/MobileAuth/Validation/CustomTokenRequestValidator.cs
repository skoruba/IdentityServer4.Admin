using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.MobileAuth.Validation
{
    public class CustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly ILogger<CustomTokenRequestValidator> _logger;

        public CustomTokenRequestValidator(
            UserManager<UserIdentity> userManager,
            ILogger<CustomTokenRequestValidator> logger)
        {

            _userManager = userManager;
            _logger = logger;
        }
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            //If Token grant_type id refresh_token we need to check weather request from mobile device with acr_values.
            if (context?.Result?.ValidatedRequest?.GrantType != null && context.Result.ValidatedRequest.GrantType == OidcConstants.GrantTypes.RefreshToken)
            {
                var raw = context.Result.ValidatedRequest.Raw;
                var acr_values = raw.ToNameValueCollection(OidcConstants.AuthorizeRequest.AcrValues);
                var subject = context.Result.ValidatedRequest.Subject ?? context.Result.ValidatedRequest.AuthorizationCode?.Subject;


                if (!string.IsNullOrEmpty(acr_values.Get("device_id")))
                {
                    if (subject != null)
                    {
                        var user = await _userManager.FindByIdAsync(subject.Claims.FirstOrDefault(x => x.Type == "sub").Value);
                        if (user != null)
                        {
                            var claims = await _userManager.GetClaimsAsync(user);

                            if (claims.Any(x => x.Type == "device_id"))
                            {
                                var devid = claims.FirstOrDefault(x => x.Type == "device_id");
                                if (devid.Value.Trim().ToLower() != acr_values.Get("device_id").Trim().ToLower())
                                {
                                    context.Result.Error = "Invalid device_id";
                                    context.Result.ErrorDescription = "Invalid device_id. or may user login different device.";
                                    context.Result.IsError = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}