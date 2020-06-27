using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Implemantation.AspNetCore
{
    public abstract class HttpTenantResolveContributorBase : TenantResolveContributorBase
    {
        public override void Resolve(ITenantResolveContext context)
        {
            var httpContext = context.ServiceProvider.GetHttpContext();
            if(httpContext == null)
            {
                return;
            }

            try
            {
                ResolveFromHttpContext(context, httpContext);
            }
            catch (Exception e)
            {
                context.ServiceProvider
                    .GetRequiredService<ILogger<HttpTenantResolveContributorBase>>()
                    .LogWarning(e.ToString());
            }
        }

        protected virtual void ResolveFromHttpContext(ITenantResolveContext context, HttpContext httpContext)
        {
            var tenantIdOrName = GetTenantIdOrNameFromHttpContextOrNull(context, httpContext);
            if (!string.IsNullOrEmpty(tenantIdOrName))
            {
                context.TenantIdOrName = tenantIdOrName;
            }
        }

        protected abstract string GetTenantIdOrNameFromHttpContextOrNull(ITenantResolveContext context, HttpContext httpContext);
    }
}
