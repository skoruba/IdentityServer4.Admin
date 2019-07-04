using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using IdentityServer4.EntityFramework.Options;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;
using System.Linq;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators;

namespace Skoruba.IdentityServer4.Admin.DependencyInjection
{
    public static partial class BuilderExtensions
    {
        // Default implementation
        public static Builder AddMultiTenantConfiguration
            (this IServiceCollection services,
            IHostingEnvironment hostingEnvironment,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions,
            Action<DbContextOptionsBuilder> logDbContextOptions,
            Action<IdentityOptions> identityOptions)
        {
            var builder = new Builder(services);
            builder.Services.AddMemoryCache();

            builder.Services.AddCustomConfiguration
                <MultiTenantUserIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>
                (hostingEnvironment, identityDbContextOptions, configurationStoreOptions, operationalStoreOptions, logDbContextOptions);

            builder.AddMultiTenantIdentity<MultiTenantUserIdentityDbContext, MultiTenantUserIdentity, UserIdentityRole>(identityOptions);

            builder.AddCustomAdminAspNetIdentityServices
                <MultiTenantUserIdentityDbContext, IdentityServerPersistedGrantDbContext, MultiTenantUserDto<string>, string,
                RoleDto<string>, string, string, string,
                MultiTenantUserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UsersDto<MultiTenantUserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>();

            builder.Services.AddMultiTenantServiceDepencies();

            return builder;
        }

        public static IBuilder AddMultiTenantIdentity
            <TIdentityDbContext, TUserIdentity, TUserIdentityRole>
            (this IBuilder builder, Action<IdentityOptions> identityOptions)
            where TIdentityDbContext : DbContext
            where TUserIdentity : class
            where TUserIdentityRole : class
        {
            builder.Services
                .AddIdentity<TUserIdentity, TUserIdentityRole>(identityOptions)
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddUserStore<MultiTenantUserStore>()
                //.AddSignInManager<MultiTenantSigninManager<TUserIdentity>>()
                .AddUserManager<MultiTenantUserManager<TUserIdentity>>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<ITenantStore, TenantStore>();
            builder.Services.AddScoped<ITenantManager, TenantManager>();

            builder.Services.Remove(builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IUserValidator<MultiTenantUserIdentity>)));
            builder.AddUserValidator<MultiTenantUserIdentity, MultiTenantUserValidator>();
            builder.AddUserValidator<MultiTenantUserIdentity, RequireTenant>();
            builder.AddUserValidator<MultiTenantUserIdentity, MightRequireTwoFactorAuthentication<MultiTenantUserIdentity>>();

            return builder;
        }
    }
}