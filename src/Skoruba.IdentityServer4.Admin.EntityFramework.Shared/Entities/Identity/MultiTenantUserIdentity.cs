using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity
{
    public class MultiTenantUserIdentity : UserIdentity
    {
        public string TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}