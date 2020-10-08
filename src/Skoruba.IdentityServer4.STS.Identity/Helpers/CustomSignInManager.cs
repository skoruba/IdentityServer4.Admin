/* Author: J. Arturo
 * Based on source code at: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/Core/src/SignInManager.cs
 */
using IdentityModel;
using IdentityServer4;
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
        private const string LoginProviderKey = "LoginProvider";
        private const string XsrfKey = "XsrfId";

        private IUserClaimsPrincipalFactory<TUser> _claimsFactory;

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
            _claimsFactory = claimsFactory;
        }

        public override async Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            List<Claim> claims = new List<Claim>();

            foreach (var claim in additionalClaims)
            {
                claims.Add(claim);
            }

            var externalResult = await _contextAccessor.HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
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

                // check authentication method reference value
                var amr = claims.FirstOrDefault(c => c.Type == "amr");
                if (amr != null && amr.Value == "mfa")
                {
                    // remove multifactor authentication claim to prevent signin issues with external providers
                    claims.Remove(amr);
                }

                await _contextAccessor.HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            }

            await base.SignInWithClaimsAsync(user, authenticationProperties, claims);
        }

        /// <summary>
        /// Overrided method to use ExternalCookieAuthenticationScheme
        /// </summary>
        /// <param name="expectedXsrf"></param>
        /// <returns></returns>
        public override async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            var auth = await _contextAccessor.HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var items = auth?.Properties?.Items;
            if (auth?.Principal == null || items == null || !items.ContainsKey(LoginProviderKey))
            {
                return null;
            }

            if (expectedXsrf != null)
            {
                if (!items.ContainsKey(XsrfKey))
                {
                    return null;
                }
                var userId = items[XsrfKey] as string;
                if (userId != expectedXsrf)
                {
                    return null;
                }
            }

            var providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = items[LoginProviderKey] as string;
            if (providerKey == null || provider == null)
            {
                return null;
            }

            var providerDisplayName = (await GetExternalAuthenticationSchemesAsync()).FirstOrDefault(p => p.Name == provider)?.DisplayName
                                      ?? provider;
            return new ExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
            {
                AuthenticationTokens = auth.Properties.GetTokens(),
                AuthenticationProperties = auth.Properties
            };
        }
    }
}