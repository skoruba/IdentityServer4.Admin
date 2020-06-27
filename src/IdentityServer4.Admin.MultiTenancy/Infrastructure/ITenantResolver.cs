namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public interface ITenantResolver
    {
        TenantResolveResult ResolveTenantIdOrName();
    }
}
