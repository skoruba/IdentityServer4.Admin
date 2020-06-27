namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public abstract class TenantResolveContributorBase : ITenantResolveContributor
    {
        public abstract string Name { get; }

        public abstract void Resolve(ITenantResolveContext context);
    }
}
