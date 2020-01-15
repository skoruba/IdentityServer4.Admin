using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Finbuckle.Strategies
{
    public class ClaimsStrategy : IMultiTenantStrategy
    {
        private readonly ILogger<ClaimsStrategy> _logger;

        public ClaimsStrategy(ILogger<ClaimsStrategy> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (!(context is HttpContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

            var httpContext = context as HttpContext;

            var tenantId = httpContext?.User?.GetTenantId();

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                var store = httpContext.RequestServices.GetRequiredService<IMultiTenantStore>();

                var tenantInfo = await store.TryGetAsync(tenantId);

                return tenantInfo.Identifier;
            }

            return null;
        }
    }
}