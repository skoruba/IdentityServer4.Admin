using System;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public interface ITenantResolveContext
    {
        IServiceProvider ServiceProvider { get; }

        string TenantIdOrName { get; set; }

        bool Handled { get; set; }
    }
}
