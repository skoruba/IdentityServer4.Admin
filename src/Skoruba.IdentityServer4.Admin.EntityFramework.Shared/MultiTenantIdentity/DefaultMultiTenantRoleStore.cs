using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.MultiTenantIdentity
{
    public class DefaultMultiTenantRoleStore : MultiTenantRoleStore<UserIdentityRole, AdminIdentityDbContext, string, UserIdentityUserRole, UserIdentityRoleClaim>
    {
        public DefaultMultiTenantRoleStore(AdminIdentityDbContext context, ISkorubaTenant skorubaMultiTenant, IdentityErrorDescriber describer = null) : base(context, skorubaMultiTenant, describer)
        {
        }
    }
}
