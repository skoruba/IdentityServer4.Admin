using IdentityServer4.Admin.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entitites.Identity
{
    public class UserIdentityUserRole : UserIdentityUserRole<string>, IMultiTenant
    {
    }

    public class UserIdentityUserRole<TKey> : IdentityUserRole<TKey>, IMultiTenant
        where TKey : IEquatable<TKey>
    {
        public Guid? TenantId { get; protected set; }
    }
}