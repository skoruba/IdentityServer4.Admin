using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public class TenantResolveContext : ITenantResolveContext
    {
        public IServiceProvider ServiceProvider { get; }

        public string TenantIdOrName { get; set; }

        public bool Handled { get; set; }

        public bool HasResolvedTenantOrHost()
        {
            return Handled || TenantIdOrName != null;
        }

        public TenantResolveContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
