using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using IdentityServer4.EntityFramework.Options;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.Api.DependencyInjection
{
    public static partial class BuilderExtensions
    {
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
                (identityDbContextOptions, configurationStoreOptions, operationalStoreOptions, logDbContextOptions)
            .AddCustomIdentity<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(identityOptions);

            builder.Services.AddAdminAspNetIdentityServices<AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<string>, string, RoleDto<string>, string, string, string,
                   UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                   UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                   UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                   UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                   RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>(ProfileTypes);

            builder.Services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

            builder.AddMvcServices<UserDto<string>, string, RoleDto<string>, string, string, string,
                UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                RoleClaimsDto<string>>();

            //Uncomment below to require 2fa
            //builder.AddUserValidator<UserIdentity, MightRequireTwoFactorAuthentication<UserIdentity>>();

            return builder;
        }
    }
}