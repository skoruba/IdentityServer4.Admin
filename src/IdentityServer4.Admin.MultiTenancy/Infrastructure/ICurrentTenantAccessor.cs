namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public interface ICurrentTenantAccessor
    {
        BasicTenantInfo Current { get; set; }
    }
}
