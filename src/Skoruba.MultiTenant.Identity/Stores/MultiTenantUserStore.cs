﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.MultiTenant.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Identity.Stores
{

    // TODO: Can MultiTenantUserStore be used regardless of tenant implementation and be renamed to UserMightHaveTenantStore?
    public abstract class MultiTenantUserStore<TUser, TRole, TContext, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> :
        UserStore<TUser, TRole, TContext, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>, IHaveTenantId
        where TRole : IdentityRole<TKey>, IHaveTenantId
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUserToken : IdentityUserToken<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        private readonly ISkorubaTenantContext _skorubaMultiTenantContext;

        protected string CurrentTenantId => _skorubaMultiTenantContext.Tenant?.Id;

        public MultiTenantUserStore(TContext context, ISkorubaTenantContext skorubaMultiTenantContext, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _skorubaMultiTenantContext = skorubaMultiTenantContext;

        }

        /// <summary>
        /// A navigation property for the users the store contains, filtered by the current tenant id (if exists).
        /// </summary>
        public override IQueryable<TUser> Users => !_skorubaMultiTenantContext.TenantResolved && !_skorubaMultiTenantContext.TenantResolutionRequired
            // return the base users (no tenant filtering) if tenant is not required
            ? base.Users
            // return users filtered on tenant if tenant is required
            : base.Users.Where(u => u.TenantId == CurrentTenantId);


        protected override Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken)
        {
            // Base.FindUserAsync uses the Users IQueryable property.
            // Base.FindUserAync is used by the FindByLoginAsync method.
            // FindByLoginAsync should not be filtered by tenant id
            // Therefore, overriding this method to use the Context
            // User set so that we do not include the tenant id
            // in the filter.
            return Context.Set<TUser>().SingleOrDefaultAsync(u => u.Id.Equals(userId), cancellationToken);
        }

        /// <summary>
        /// Return a role for the current tenant with the normalized name if it exists.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The role if it exists.</returns>
        protected override Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            // Unfortuantely the base method utilizes a
            // private member Roles which cannot be overriden

            return !_skorubaMultiTenantContext.TenantResolved && !_skorubaMultiTenantContext.TenantResolutionRequired
                ? base.FindRoleAsync(normalizedRoleName, cancellationToken)
                : Context.Set<TRole>()
                    .Where(r => r.TenantId == CurrentTenantId)
                    .SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);
        }
    }
}
