using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;

namespace Skoruba.IdentityServer4.STS.Identity.Services
{
    /// <summary>
    /// Adds additional tenant specific claims.  <seealso cref="PrincipalExtensions"/>
    /// </summary>
    public class MultiTenantProfileService : IProfileService
    {
        /// <summary>
        /// The claims factory.
        /// </summary>
        protected readonly IUserClaimsPrincipalFactory<MultiTenantUserIdentity> ClaimsFactory;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger<MultiTenantProfileService> Logger;

        /// <summary>
        /// The user manager.
        /// </summary>
        protected readonly UserManager<MultiTenantUserIdentity> UserManager;

        private readonly ITenantManager _tenantManager;

        public MultiTenantProfileService(UserManager<MultiTenantUserIdentity> userManager,
            ITenantManager tenantManager,
            IUserClaimsPrincipalFactory<MultiTenantUserIdentity> claimsFactory,
            ILogger<MultiTenantProfileService> logger)
        {
            UserManager = userManager;
            _tenantManager = tenantManager;
            ClaimsFactory = claimsFactory;
            Logger = logger;
        }

        //TODO: Convert strings to consts and localize
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await UserManager.FindByIdAsync(sub);
            if (user == null)
            {
                Logger?.LogWarning("No user found matching subject Id: {0}", sub);
            }
            else
            {
                var principal = await ClaimsFactory.CreateAsync(user);
                if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

                List<Claim> claims = new List<Claim>();

                if (!string.IsNullOrEmpty(user.TenantId))
                {
                    var tenant = await _tenantManager.FindByIdFromCacheAsync(user.TenantId);
                    claims.Add(new Claim("tenantid", user.TenantId));
                    claims.Add(new Claim("tenantname", tenant.Name));
                    claims.Add(new Claim("dbname", tenant.DatabaseName));
                    // claims with null values throw errors
                    if (!string.IsNullOrWhiteSpace(user.ApplicationId))
                    {
                        claims.Add(new Claim("applicationid", user.ApplicationId));
                    }
                }
                context.AddRequestedClaims(principal.Claims);
                context.AddRequestedClaims(claims);
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No subject Id claim present");

            var user = await UserManager.FindByIdAsync(sub);
            if (user == null)
            {
                Logger?.LogWarning("No user found matching subject Id: {0}", sub);
            }

            context.IsActive = user != null;
        }
    }
}