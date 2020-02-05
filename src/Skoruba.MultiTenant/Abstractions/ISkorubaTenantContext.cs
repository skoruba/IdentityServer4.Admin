namespace Skoruba.MultiTenant.Abstractions
{
    public interface ISkorubaTenantContext
    {
        ISkorubaTenant Tenant { get; }
        bool MultiTenantEnabled { get; }
        bool TenantResolved { get; }
        bool TenantResolutionRequired { get; }
        string TenantResolutionStrategy { get; }
        void SetTenantId(IHaveTenantId obj);
    }
}
