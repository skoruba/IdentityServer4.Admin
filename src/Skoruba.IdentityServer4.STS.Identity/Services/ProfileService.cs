using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Services
{
    public class ProfileService : ProfileService<UserIdentity>
    {
        protected UserManager<UserIdentity> _userManager;
        protected readonly ILogger<ProfileService<UserIdentity>> _logger;
        public ProfileService(
            UserManager<UserIdentity> userManager,
            IUserClaimsPrincipalFactory<UserIdentity> claimsFactory,
            ILogger<ProfileService<UserIdentity>> logger)
            : base(userManager, claimsFactory)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await _userManager.GetUserAsync(context.Subject);
            if (user == null)
            {
                _logger?.LogWarning("No user found matching subject Id: {0}", sub);
            }
            else
            {
                var principal = await ClaimsFactory.CreateAsync(user);
                if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

                context.AddRequestedClaims(principal.Claims);

                if (user.TenantId.HasValue)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(IdentityClaimTypes.TenantId, user.TenantId.ToString()),
                    };
                    context.AddRequestedClaims(claims);
                }
            }
        }

        public override async Task IsActiveAsync(IsActiveContext context)
        {
            await base.IsActiveAsync(context);
        }
    }
}
