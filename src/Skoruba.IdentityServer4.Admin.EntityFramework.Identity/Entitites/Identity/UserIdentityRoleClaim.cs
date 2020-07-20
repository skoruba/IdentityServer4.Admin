using IdentityServer4.Admin.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entitites.Identity
{
    public class UserIdentityRoleClaim : UserIdentityRoleClaim<string>, IMultiTenant
    {
    }

    public class UserIdentityRoleClaim<TKey> : IdentityRoleClaim<TKey>, IMultiTenant
        where TKey : IEquatable<TKey>
    {
        public Guid? TenantId { get; protected set; }
    }
}