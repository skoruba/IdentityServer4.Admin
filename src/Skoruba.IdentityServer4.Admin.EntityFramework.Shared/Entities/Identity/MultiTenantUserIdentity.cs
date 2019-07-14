using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity
{
    public class MultiTenantUserIdentity : UserIdentity
    {
        /// <summary>
        /// Gets or sets the id of the tenant that this user belongs to
        /// </summary>
        public string TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }

        /// <summary>
        /// Gets or sets the users application id
        /// </summary>
        public string ApplicationId { get; set; }
    }
}