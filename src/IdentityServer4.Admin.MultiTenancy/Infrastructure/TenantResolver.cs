using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public TenantResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public TenantResolveResult ResolveTenantIdOrName()
        {
            var result = new TenantResolveResult();
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var context = new TenantResolveContext(serviceScope.ServiceProvider);

                foreach (var tenantResolver in serviceScope.ServiceProvider.GetServices<ITenantResolveContributor>())
                {
                    tenantResolver.Resolve(context);
                    result.AppliedResolvers.Add(tenantResolver.Name);
                    if (context.HasResolvedTenantOrHost())
                    {
                        result.TenantIdOrName = context.TenantIdOrName;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
