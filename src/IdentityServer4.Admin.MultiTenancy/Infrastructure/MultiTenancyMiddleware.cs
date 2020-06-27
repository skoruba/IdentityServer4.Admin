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

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
