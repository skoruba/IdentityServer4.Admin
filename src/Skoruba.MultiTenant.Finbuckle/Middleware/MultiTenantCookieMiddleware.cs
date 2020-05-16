using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Skoruba.MultiTenant.Configuration;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Finbuckle.Middleware
{
    public class MultiTenantCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public MultiTenantCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, 
            IMultiTenantStore store, 
            MultiTenantConfiguration multiTenantConfiguration,
            Skoruba.MultiTenant.Finbuckle.Configuration.Configuration finbuckleConfiguration)
        {
            if (multiTenantConfiguration.MultiTenantEnabled)
            {
#pragma warning disable CS0162 // Unreachable code detected
                var multiTenantContext = context.GetMultiTenantContext();
#pragma warning restore CS0162 // Unreachable code detected

                bool resolveTenant = multiTenantContext?.TenantInfo == null;

                // If the tenant has already been resolved then skip
                if (resolveTenant)
                {
                    TenantInfo tenantInfo = null;

                    var tenantId = context.Request?.Cookies[finbuckleConfiguration.TenantKey];
                    if (!string.IsNullOrWhiteSpace(tenantId))
                    {
                        tenantInfo = await store.TryGetAsync(tenantId);
                    }
                    if (tenantInfo != null)
                    {
                        context.TrySetTenantInfo(tenantInfo, true);
                    }
                }
            }
            if (_next != null)
            {
                await _next(context);
            }
        }
    }
}