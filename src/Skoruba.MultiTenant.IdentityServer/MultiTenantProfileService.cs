using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Skoruba.MultiTenant.Abstractions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.IdentityServer
{
    public class MultiTenantProfileService<TUser> : ProfileService<TUser>
         where TUser : class
    {
        public MultiTenantProfileService(UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory, ILogger<ProfileService<TUser>> logger) : base(userManager, claimsFactory, logger)
        {
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await UserManager.FindByIdAsync(sub);

            Claim tenantClaim = null;

            if (user is IHaveTenantId)
            {
                tenantClaim = new Claim(Claims.ClaimTypes.TenantId, ((IHaveTenantId)user).TenantId);
            }
            if (user == null)
            {
                Logger?.LogWarning("No user found matching subject Id: {0}", sub);
            }
            else
            {
                var principal = await ClaimsFactory.CreateAsync(user);

                if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

                var claims = principal.Claims;

                if (tenantClaim != null)
                {
                    claims.Union(new[] { tenantClaim });
                }

                context.AddRequestedClaims(claims);
            }
        }
    }
}