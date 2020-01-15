using Microsoft.AspNetCore.Identity;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddMultiTenantServicesIfMultiTenant<TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>(this IdentityBuilder builder)
            where TUserIdentity : class 
            where TUserIdentityRole : class
            where TRoleStore : class
            where TUserStore : class
        {
            if (MultiTenantConstants.MultiTenantEnabled)
            {
                builder.AddClaimsPrincipalFactory<MultiTenantUserClaimsPrincipalFactory<TUserIdentity, TUserIdentityRole>>();
                builder.AddRoleStore<TRoleStore>();
                builder.AddUserStore<TUserStore>();
            }
            return builder;
        }
    }
}
