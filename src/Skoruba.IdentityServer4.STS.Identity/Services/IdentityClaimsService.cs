using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Skoruba.IdentityServer4.STS.Identity.Services
{
    public class IdentityClaimsService : DefaultClaimsService
    {
        public IdentityClaimsService(IProfileService profile, ILogger<IdentityClaimsService> logger)
            : base(profile, logger)
        {

        }

        protected override IEnumerable<Claim> GetOptionalClaims(ClaimsPrincipal subject)
        {
            var tenantClaim = subject.FindFirst(IdentityClaimTypes.TenantId);
            if (tenantClaim == null)
            {
                return base.GetOptionalClaims(subject);
            }

            return base.GetOptionalClaims(subject).Union(new[] { tenantClaim });
        }
    }
}
