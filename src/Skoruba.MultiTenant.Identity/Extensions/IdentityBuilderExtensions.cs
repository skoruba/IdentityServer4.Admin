using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Identity;
using Skoruba.MultiTenant.Identity.Validators;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddDefaultMultiTenantIdentityServices<TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>(this IdentityBuilder builder, bool isMultiTenantEnabled)
            where TUserIdentity : class
            where TUserIdentityRole : class
            where TRoleStore : class
            where TUserStore : class
        {            
            if (isMultiTenantEnabled)
            {
                builder.AddRoleStore<TRoleStore>();
                builder.AddUserStore<TUserStore>();
                builder.AddClaimsPrincipalFactory<MultiTenantUserClaimsPrincipalFactory<TUserIdentity, TUserIdentityRole>>();
                builder.AddUserValidator<TUserIdentity, UserRequiresTenantIdValidator<TUserIdentity>>();
                builder.AddUserValidator<TUserIdentity, UserRequiresTwoFactorAuthenticationValidator<TUserIdentity>>();
                builder.AddRoleValidator<TUserIdentityRole, RoleRequiresTenantIdValidator<TUserIdentityRole>>();
            }
            return builder;
        }
        public static IdentityBuilder AddMultiTenantIdentityStores<TUserIdentity, TUserIdentityRole, TUserStore, TRoleStore>(this IdentityBuilder builder, bool isMultiTenantEnabled)
          where TUserIdentity : class
          where TUserIdentityRole : class
          where TRoleStore : class
          where TUserStore : class
        {
            if (isMultiTenantEnabled)
            {
                builder.AddRoleStore<TRoleStore>();
                builder.AddUserStore<TUserStore>();
            }
            return builder;
        }

        public static IdentityBuilder AddUserValidator<TUser, TValidator>(this IdentityBuilder builder)
           where TUser : class
           where TValidator : class, IUserValidator<TUser>
        {
            builder.Services.AddScoped(typeof(IUserValidator<TUser>), typeof(TValidator));
            return builder;
        }
        public static IdentityBuilder AddRoleValidator<TRole, TValidator>(this IdentityBuilder builder)
           where TRole : class
           where TValidator : class, IRoleValidator<TRole>
        {
            builder.Services.AddScoped(typeof(IRoleValidator<TRole>), typeof(TValidator));
            return builder;
        }
    }
}
