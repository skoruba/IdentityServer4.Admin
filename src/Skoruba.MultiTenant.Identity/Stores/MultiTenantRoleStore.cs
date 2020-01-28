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
    // TODO: Can MultiTenantRoleStore be used regardless of tenant implementation and be renamed to RoleMightHaveTenantStore?
    public abstract class MultiTenantRoleStore<TRole, TContext, TKey, TUserRole, TRoleClaim> :
        RoleStore<TRole, TContext, TKey, TUserRole, TRoleClaim>
        where TRole : IdentityRole<TKey>, IHaveTenantId
        where TKey : IEquatable<TKey>
        where TContext : DbContext
        where TUserRole : IdentityUserRole<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        private readonly ISkorubaTenant _skorubaMultiTenant;
        protected string CurrentTenantId => _skorubaMultiTenant.Id;

        public MultiTenantRoleStore(TContext context, ISkorubaTenant skorubaMultiTenant, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _skorubaMultiTenant = skorubaMultiTenant;
        }

        public override IQueryable<TRole> Roles => _skorubaMultiTenant == null && !_skorubaMultiTenant.TenantResolutionRequired
            // return the roles not filtered if tenant is not required
            ? base.Roles
            // return roles filtered on tenant if tenant is required
            : base.Roles.Where(r => r.TenantId == CurrentTenantId);

        public override Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            if (!_skorubaMultiTenant.TenantResolved && !_skorubaMultiTenant.TenantResolutionRequired)
            {
                return base.FindByNameAsync(normalizedName, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName && r.TenantId == CurrentTenantId, cancellationToken);
        }
    }
}
