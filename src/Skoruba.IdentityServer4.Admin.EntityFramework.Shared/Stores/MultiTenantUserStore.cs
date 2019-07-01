using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores
{
    public class MultiTenantUserStore :
        UserStore<MultiTenantUserIdentity, UserIdentityRole, MultiTenantUserIdentityDbContext, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityUserToken, UserIdentityRoleClaim>,
        IMultiTenantUserStore<MultiTenantUserIdentity>
    {
        public MultiTenantUserStore(MultiTenantUserIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        [Obsolete]
        public override Task<MultiTenantUserIdentity> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            return null;
        }

        [Obsolete]
        public override Task<MultiTenantUserIdentity> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            return null;
        }

        public async Task<MultiTenantUserIdentity> FindByEmailAsync(string tenantCode, string normalizedEmail, CancellationToken cancellationToken = default)
        {
            var user = await (Users as IQueryable<MultiTenantUserIdentity>)
                .SingleOrDefaultAsync(a => a.Tenant.Code == tenantCode && a.NormalizedEmail == normalizedEmail, cancellationToken);

            return user;
        }

        public async Task<MultiTenantUserIdentity> FindByNameAsync(string tenantCode, string normalizedUserName, CancellationToken cancellationToken = default)
        {
            var user = await (Users as IQueryable<MultiTenantUserIdentity>)
                .SingleOrDefaultAsync(a => a.Tenant.Code == tenantCode && a.NormalizedUserName == normalizedUserName, cancellationToken);
            return user;
        }
    }
}