using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Implemantation.AspNetCore
{
    public class HeaderTenantResolveContributor : HttpTenantResolveContributorBase
    {
        public override string Name => "Header";

        protected override string GetTenantIdOrNameFromHttpContextOrNull(ITenantResolveContext context, HttpContext httpContext)
        {
            if (httpContext.Request == null || httpContext.Request.Headers == null || httpContext.Request.Headers.Count <= 0)
            {
                return null;
            }

            var tenantKey = MultiTenancyConsts.TenantKey;
            var tenantHeader = httpContext.Request.Headers[tenantKey];
            if (tenantHeader == string.Empty || tenantHeader.Count < 1)
            {
                return null;
            }

            if (tenantHeader.Count > 1)
            {
                context
                .ServiceProvider
                .GetRequiredService<ILogger<HeaderTenantResolveContributor>>()
                .LogWarning($"HTTP request includes more than one {tenantKey} header value. First one will be used. All of them: {string.Join(", ", tenantHeader)}");
            }
            context.Handled = true;
            return tenantHeader.First();
        }
    }
}
