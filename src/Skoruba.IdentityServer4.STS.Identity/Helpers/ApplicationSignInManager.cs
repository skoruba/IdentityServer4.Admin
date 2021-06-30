// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// File: https://github.com/IdentityServer/IdentityServer4/blob/main/samples/Quickstarts/3_AspNetCoreAndApis/src/IdentityServer/Quickstart/Account/ExternalController.cs

// Modified by Jan Škoruba and J. Arturo

using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class ApplicationSignInManager<TUser> : SignInManager<TUser>
        where TUser : class
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly LdapWebApiProviderConfiguration<TUser> _ldapWebApiProvider;

        public ApplicationSignInManager(UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<TUser> confirmation,
            LdapWebApiProviderConfiguration<TUser> ldapWebApiProvider) : base(userManager, contextAccessor,
                claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _contextAccessor = contextAccessor;
            _ldapWebApiProvider = ldapWebApiProvider;
        }

        public override async Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            var claims = additionalClaims.ToList();

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
                    // if the external provider issued an id_token, we'll keep it for sign out
                    var idToken = externalResult.Properties.GetTokenValue("id_token");
                    if (idToken != null)
                    {
                        authenticationProperties.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
                    }
                }

                var authenticationMethod = claims.FirstOrDefault(x => x.Type == ClaimTypes.AuthenticationMethod);
                var idp = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IdentityProvider);

                if (authenticationMethod != null && idp == null)
                {
                    claims.Add(new Claim(JwtClaimTypes.IdentityProvider, authenticationMethod.Value));
                }
            }

            await base.SignInWithClaimsAsync(user, authenticationProperties, claims);
        }

        public override async Task<SignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure)
        {
            var userDomainProfile = _ldapWebApiProvider.GetUserDomainProfile(user);

            //The user does not have a Domain assigned. 
            if (userDomainProfile == null)
                return await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);

            var userIdentity = (user as Admin.EntityFramework.Shared.Entities.Identity.UserIdentity);

            var accountName = userIdentity.UserName.Split('\\').GetValue(1).ToString();

            var accountSecurityData = new Bitai.LDAPWebApi.DTO.LDAPAccountCredentials
            {
                DomainName = (user as Admin.EntityFramework.Shared.Entities.Identity.UserIdentity).UserDomain,
                AccountName = accountName,
                AccountPassword = password
            };

            var ldapCredentialsClient = userDomainProfile.GetLdapCredentialsClient();
            var response = await ldapCredentialsClient.AccountAuthenticationAsync(accountName, accountSecurityData);
            if (!response.IsSuccessResponse)
            {
                return SignInResult.NotAllowed;
            }
            else
            {
                return SignInResult.Success;
            }
        }
    }
}

