using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using IdentityServer4.EntityFramework.Options;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators;

namespace Skoruba.IdentityServer4.Admin.DependencyInjection
{
    public static partial class BuilderExtensions
    {
        // Default implementation
        public static Builder AddSingleTenantConfiguration
            (this IServiceCollection services,
            IHostingEnvironment hostingEnvironment,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions,
            Action<DbContextOptionsBuilder> logDbContextOptions,
            Action<IdentityOptions> identityOptions)
        {
            var builder = new Builder(services);

            builder.Services.AddCustomConfiguration
                <AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>
                (hostingEnvironment, identityDbContextOptions, configurationStoreOptions, operationalStoreOptions, logDbContextOptions);

            builder.AddSingleTenantIdentity<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(identityOptions);

            builder.AddCustomAdminAspNetIdentityServices<AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<string>, string, RoleDto<string>, string, string, string,
                UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>();

            //Uncomment below to require 2fa
            //builder.AddUserValidator<UserIdentity, MightRequireTwoFactorAuthentication<UserIdentity>>();

            return builder;
        }

        public static IBuilder AddSingleTenantIdentity
            <TIdentityDbContext, TUserIdentity, TUserIdentityRole>
            (this IBuilder builder, Action<IdentityOptions> identityOptions)
            where TIdentityDbContext : DbContext
            where TUserIdentity : class
            where TUserIdentityRole : class
        {
            builder.Services
                .AddIdentity<TUserIdentity, TUserIdentityRole>(identityOptions)
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddDefaultTokenProviders();

            return builder;
        }
    }
}