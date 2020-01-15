namespace Skoruba.MultiTenant.Abstractions
{
    public interface IValidateTenantRequirement
    {
        bool TenantIsRequired();
    }
}
