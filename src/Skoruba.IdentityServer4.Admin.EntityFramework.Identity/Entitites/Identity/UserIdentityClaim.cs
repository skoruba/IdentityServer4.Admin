using IdentityServer4.Admin.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entitites.Identity
{
    public class UserIdentityUserClaim : UserIdentityUserClaim<string>, IMultiTenant
    {
    }

    public class UserIdentityUserClaim<TKey> : IdentityUserClaim<TKey>, IMultiTenant
        where TKey : IEquatable<TKey>
    {
        public Guid? TenantId { get; protected set; }
    }
}
