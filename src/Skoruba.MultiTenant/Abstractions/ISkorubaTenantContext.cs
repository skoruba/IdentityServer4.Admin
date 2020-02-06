using Skoruba.MultiTenant.Configuration;

namespace Skoruba.MultiTenant.Abstractions
{
    public interface ISkorubaTenantContext
    {
        ISkorubaTenant Tenant { get; }
        bool TenantResolved { get; }
        bool TenantResolutionRequired { get; }
        string TenantResolutionStrategy { get; }
        MultiTenantConfiguration Configuration { get; }
        void SetTenantId(IHaveTenantId obj);
    }
}
