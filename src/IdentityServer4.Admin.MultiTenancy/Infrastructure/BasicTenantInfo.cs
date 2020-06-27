using System;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public class BasicTenantInfo
    {
        /// <summary>
        /// Null indicates the host.
        /// Not null value for a tenant.
        /// </summary>
        public Guid? TenantId { get; }

        /// <summary>
        /// Name of the tenant if <see cref="TenantId"/> is not null.
        /// </summary>
        public string Name { get; }

        public BasicTenantInfo(Guid? tenantId, string name = null)
        {
            TenantId = tenantId;
            Name = name;
        }
    }
}
