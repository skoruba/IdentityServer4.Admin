using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Implemantation.AspNetCore
{
    public class DomainTenantResolveContributor : HttpTenantResolveContributorBase
    {
        public override string Name => "Domain";

        protected override string GetTenantIdOrNameFromHttpContextOrNull(ITenantResolveContext context, HttpContext httpContext)
        {
            if (httpContext.Request?.Host == null)
            {
                return null;
            }
            var host = httpContext.Request.Host.Host;
            host = RemovePrefix(host);

            if (host.Split('.').Length > 2)
            {
                context.Handled = true;

                var lastIndex = host.LastIndexOf(".");
                var index = host.LastIndexOf(".", lastIndex - 1);
                return host.Substring(0, index);
            }
            return null;
        }

        private string RemovePrefix(string host)
        {
            if (host.StartsWith("https://"))
            {
                return host.Replace("https://", "");
            }

            if (host.StartsWith("http://"))
            {
                return host.Replace("http://", "");
            }

            return host;
        }
    }
}
