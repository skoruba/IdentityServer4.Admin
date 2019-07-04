using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class CheckIf2faCanBeDisabledHandler : AuthorizationHandler<CheckIf2faCanBeDisabledRequirement>
    {
        private readonly ITenantManager _tenantManager;
        private readonly ClaimsPrincipal _principal;

        public CheckIf2faCanBeDisabledHandler(IEnumerable<ITenantManager> tenantManager, IHttpContextAccessor httpContext)
        {
            _tenantManager = tenantManager.FirstOrDefault();
            _principal = httpContext.HttpContext.User;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckIf2faCanBeDisabledRequirement requirement)
        {
            if (_principal == null || !_principal.IsAuthenticated())
            {
                return;
            }

            if (_tenantManager == null)
            {
                context.Succeed(requirement);
            }

            var userTenantId = _principal.GetTenantId();
            if (userTenantId == null)
            {
                context.Succeed(requirement);
            }

            var tenant = await _tenantManager.FindByIdFromCacheAsync(userTenantId);
            if (!tenant.RequireTwoFactorAuthentication)
            {
                context.Succeed(requirement);
            }
        }
    }
}