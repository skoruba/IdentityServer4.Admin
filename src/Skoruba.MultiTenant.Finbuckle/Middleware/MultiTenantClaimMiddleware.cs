using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Claims;

namespace Skoruba.MultiTenant.Finbuckle.Middleware
{
    /// <summary>
    /// Middleware for resolving the TenantContext using the <see cref="ClaimsPrincipal"/>
    /// for <see cref="ClaimTypes.TenantId"/>.  This middleware
    /// will NOT override any existing TenantContext previously set.
    /// </summary>
    public class MultiTenantClaimMiddleware
    {
        private readonly RequestDelegate _next;

        public MultiTenantClaimMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IMultiTenantStore store)
        {

            if (MultiTenantConstants.MultiTenantEnabled)
            {
#pragma warning disable CS0162 // Unreachable code detected
                var multiTenantContext = context.GetMultiTenantContext();
#pragma warning restore CS0162 // Unreachable code detected

                bool resolveTenant = multiTenantContext?.TenantInfo == null;

                // If the tenant has already been resolved then skip
                if (resolveTenant)
                {
                    // TODO: Add options for handling what to do when the tenant is already set, eg replace, skip, throw
                    var tenantId = context?.User?.GetTenantId();

                    if (!string.IsNullOrWhiteSpace(tenantId))
                    {
                        var tenantInfo = await store.TryGetAsync(tenantId);

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