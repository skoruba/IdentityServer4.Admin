using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Implemantation.AspNetCore
{
    public class ClaimsTenantResolveContributor : TenantResolveContributorBase
    {
        public override string Name => "Claims";

        public override void Resolve(ITenantResolveContext context)
        {
            var httpContextAccessor = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            var claimsPrincipal = httpContextAccessor.HttpContext?.User;
            if(claimsPrincipal == null)
            {
                return;
            }
            var tenantId = FindTenantId(claimsPrincipal);
            if(tenantId == null)
            {
                return;
            }

            context.Handled = true;
            context.TenantIdOrName = tenantId.Value.ToString();
        }

        private Guid? FindTenantId(ClaimsPrincipal principal)
        {
            var tenantIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == "");
            if (tenantIdOrNull == null || string.IsNullOrWhiteSpace(tenantIdOrNull.Value))
            {
                return null;
            }

            return Guid.Parse(tenantIdOrNull.Value);
        }
    }
}
