using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using IdentityServer4.EntityFramework.Options;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using System.Linq;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators;

namespace Skoruba.IdentityServer4.Admin.Api.DependencyInjection
{
    public static partial class BuilderExtensions
    {
        public static IBuilder AddMultiTenantConfiguration
            (this IServiceCollection services,
            IHostingEnvironment hostingEnvironment,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions,
            Action<DbContextOptionsBuilder> logDbContextOptions,
            Action<IdentityOptions> identityOptions)
        {
            var builder = services
                 .AddCustomConfiguration
                     <MultiTenantUserIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>
                     (identityDbContextOptions, configurationStoreOptions, operationalStoreOptions, logDbContextOptions)
                 .AddCustomIdentity
                     <MultiTenantUserIdentityDbContext, MultiTenantUserIdentity, UserIdentityRole>
                     (identityOptions);

            builder.Services.AddAdminAspNetIdentityServices<MultiTenantUserIdentityDbContext, IdentityServerPersistedGrantDbContext,
                   MultiTenantUserDto<string>, string, RoleDto<string>, string, string, string,
                   MultiTenantUserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                   UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                   UsersDto<MultiTenantUserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                   UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                   RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>(ProfileTypes);

            builder.Services.Remove(builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IUserValidator<MultiTenantUserIdentity>)));
            builder.AddUserValidator<MultiTenantUserIdentity, MultiTenantUserValidator>();
            builder.AddUserValidator<MultiTenantUserIdentity, RequireTenant>();

            builder.Services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

            builder.AddMvcServices<MultiTenantUserDto<string>, string, RoleDto<string>, string, string, string,
                UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UsersDto<MultiTenantUserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                RoleClaimsDto<string>>();

            return builder;
        }
    }
}