using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public class MultiTenancyMiddleware : IMiddleware
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly ITenantStore _tenantStore;
        private readonly ITenantResolver _tenantResolver;

        public MultiTenancyMiddleware(ICurrentTenant currentTenant,
            ITenantStore tenantStore)
        {
            _currentTenant = currentTenant;
            _tenantStore = tenantStore;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var resolveResult = _tenantResolver.ResolveTenantIdOrName();
            TenantConfiguration tenant = null;
            if (resolveResult.TenantIdOrName != null)
            {
                tenant = await FindTenantAsync(resolveResult.TenantIdOrName);
                if(tenant == null)
                {
                    throw new Exception("There is no tenant with the tenant id or name: " + resolveResult.TenantIdOrName);
                }
            }

            using (_currentTenant.Change(tenant?.Id, tenant?.Name))
            {
                await next(context);
            }
        }

        private async Task<TenantConfiguration> FindTenantAsync(string tenantIdOrName)
        {
            if (Guid.TryParse(tenantIdOrName, out var parsedTenantId))
            {
                return await _tenantStore.FindAsync(parsedTenantId);
            }
            else
            {
                return await _tenantStore.FindAsync(tenantIdOrName);
            }
        }
    }
}
