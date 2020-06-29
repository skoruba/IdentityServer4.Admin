using IdentityServer4.Admin.MultiTenancy;
using Skoruba.AuditLogging.EntityFramework.Entities;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities
{
    public class AppAuditLog : AuditLog, IMultiTenant
    {
        public Guid? TenantId { get; protected set; }
    }
}
