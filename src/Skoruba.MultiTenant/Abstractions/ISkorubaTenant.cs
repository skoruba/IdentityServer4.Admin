using System.Collections.Generic;

namespace Skoruba.MultiTenant.Abstractions
{
    public interface ISkorubaTenant
    {
        string Id { get; }
        string Identifier { get; }
        string Name { get; }
        string ConnectionString { get; }
        IDictionary<string, object> Items { get; }
        bool TenantResolved { get; }
        bool TenantResolutionRequired { get; }
    }
}
