using System.Collections.Generic;

namespace Skoruba.MultiTenant.Abstractions
{
    public interface ISkorubaT
    {
        string Id { get; }
        string Identifier { get; }
        string Name { get; }
        string ConnectionString { get; }
        IDictionary<string, object> Items { get; }
    }

    public interface ISkorubaTenantContext
    {
        ISkorubaT Tenant { get; }
        bool MultiTenantEnabled { get; }
        bool TenantResolved { get; }
        bool TenantResolutionRequired { get; }
        string TenantResolutionStrategy { get; }
    }
}
