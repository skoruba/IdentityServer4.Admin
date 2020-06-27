using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace IdentityServer4.Admin.MultiTenancy.Implemantation.AspNetCore
{
    public class CookieTenantResolveContributor : HttpTenantResolveContributorBase
    {
        public override string Name => "Cookie";

        protected override string GetTenantIdOrNameFromHttpContextOrNull(ITenantResolveContext context, HttpContext httpContext)
        {
            context.Handled = true;
            return httpContext.Request?.Cookies[MultiTenancyConsts.TenantKey];
        }
    }
}
