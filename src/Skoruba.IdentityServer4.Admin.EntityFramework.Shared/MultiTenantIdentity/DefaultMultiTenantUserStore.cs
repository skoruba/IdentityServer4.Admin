using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Identity.Stores;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.MultiTenantIdentity
{
    public class DefaultMultiTenantUserStore : MultiTenantUserStore<UserIdentity, UserIdentityRole, AdminIdentityDbContext, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityUserToken, UserIdentityRoleClaim>
    {
        public DefaultMultiTenantUserStore(AdminIdentityDbContext context, ISkorubaTenant skorubaMultiTenant, IdentityErrorDescriber describer = null) : base(context, skorubaMultiTenant, describer)
        {
        }
    }
}
