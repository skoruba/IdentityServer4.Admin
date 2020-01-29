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
        private readonly ISkorubaTenantContext _skorubaMultiTenantContext;
        protected string CurrentTenantId => _skorubaMultiTenantContext.Tenant?.Id;

        public MultiTenantRoleStore(TContext context, ISkorubaTenantContext skorubaMultiTenantContext, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _skorubaMultiTenantContext = skorubaMultiTenantContext;
        }

        public override IQueryable<TRole> Roles => _skorubaMultiTenantContext == null && !_skorubaMultiTenantContext.TenantResolutionRequired
            // return the roles not filtered if tenant is not required
            ? base.Roles
            // return roles filtered on tenant if tenant is required
            : base.Roles.Where(r => r.TenantId == CurrentTenantId);

        public override Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            if (!_skorubaMultiTenantContext.TenantResolved && !_skorubaMultiTenantContext.TenantResolutionRequired)
            {
                return base.FindByNameAsync(normalizedName, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName && r.TenantId == CurrentTenantId, cancellationToken);
        }
    }
}
