using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class CustomSignInManager<TUser> : SignInManager<TUser>
        where TUser : class
    {
        private IHttpContextAccessor _contextAccessor;

        public CustomSignInManager(UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<TUser> confirmation) : base(userManager, contextAccessor,
                claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _contextAccessor = contextAccessor;
        }

        public override async Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (var claim in additionalClaims)
            {
                claims.Add(claim);
            }

            var externalResult = await _contextAccessor.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (externalResult != null && externalResult.Succeeded)
            {
                var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
                if (sid != null)
                {
                    claims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
                }

                if (authenticationProperties != null)
                {
                    // if the external provider issued an id_token, we'll keep it for signout
                    var idToken = externalResult.Properties.GetTokenValue("id_token");
                    if (idToken != null)
                    {
                        authenticationProperties.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
                    }
                }
            }

            await base.SignInWithClaimsAsync(user, authenticationProperties, claims);
        }
    }
}

