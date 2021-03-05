using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.UI.Helpers;
using System;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Shared.Configuration.Helpers;
using static Skoruba.IdentityServer4.Admin.UI.Helpers.StartupHelpers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AdminUIServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Skoruba IdentityServer4 Admin UI with the default entity model.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4AdminUI<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TAuditLog, TDataProtectionDbContext>(this IServiceCollection services, Action<IdentityServer4AdminUIOptions> optionsAction) 
            where TIdentityDbContext : IdentityDbContext<IdentityUser<string>, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLogDbContext : DbContext, IAuditLoggingDbContext<TAuditLog>
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
            where TAuditLog : AuditLog, new()
            => AddIdentityServer4AdminUI<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TAuditLog, TDataProtectionDbContext, IdentityUser<string>, IdentityRole, IdentityUserClaim<string>,
                IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>,
                IdentityUserToken<string>, string,
                UserDto<string>, RoleDto<string>, UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>,
                UserRolesDto<RoleDto<string>, string>, UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>,
                UserChangePasswordDto<string>, RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>(services, optionsAction);

        /// <summary>
        /// Adds the Skoruba IdentityServer4 Admin UI with a custom user model and database context.
        /// </summary>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4AdminUI<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TAuditLog, TDataProtectionDbContext, TUser>(this IServiceCollection services, Action<IdentityServer4AdminUIOptions> optionsAction)
            where TIdentityDbContext : IdentityDbContext<TUser, IdentityRole, string, IdentityUserClaim<string>,
                IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>,
                IdentityUserToken<string>>
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLogDbContext : DbContext, IAuditLoggingDbContext<TAuditLog>
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
            where TUser : IdentityUser<string>
            where TAuditLog : AuditLog, new()
            => AddIdentityServer4AdminUI<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TAuditLog, TDataProtectionDbContext, TUser, IdentityRole, IdentityUserClaim<string>,
                IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>,
                IdentityUserToken<string>, string, UserDto<string>, RoleDto<string>, UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>,
                UserRolesDto<RoleDto<string>, string>, UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>,
                UserChangePasswordDto<string>, RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>(services, optionsAction);

        /// <summary>
        /// Adds the Skoruba IdentityServer4 Admin UI with a fully custom entity model and database contexts.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4AdminUI<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TAuditLog, TDataProtectionDbContext, TUser, TRole, TUserClaim,
            TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey, TUserDto, TRoleDto, TUsersDto, TRolesDto, TUserRolesDto,
            TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto,
            TRoleClaimDto>
            (this IServiceCollection services, Action<IdentityServer4AdminUIOptions> optionsAction)
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLogDbContext : DbContext, IAuditLoggingDbContext<TAuditLog>
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TKey : IEquatable<TKey>
            where TUserDto : UserDto<TKey>, new()
            where TRoleDto : RoleDto<TKey>, new()
            where TUsersDto : UsersDto<TUserDto, TKey>
            where TRolesDto : RolesDto<TRoleDto, TKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
            where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
            where TUserProviderDto : UserProviderDto<TKey>
            where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TKey>
            where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
            where TUserClaimDto : UserClaimDto<TKey>
            where TRoleClaimDto : RoleClaimDto<TKey>
            where TAuditLog : AuditLog, new()
        {
            // Builds the options from user preferences or configuration.
            var options = new IdentityServer4AdminUIOptions();
            optionsAction(options);

            // Adds root configuration to the DI.
            services.AddSingleton(options.Admin);
            services.AddSingleton(options.IdentityServerData);
            services.AddSingleton(options.IdentityData);

            // Add DbContexts for Asp.Net Core Identity, Logging and IdentityServer - Configuration store and Operational store
            if (!options.Testing.IsStaging)
            {
                services.RegisterDbContexts<TIdentityDbContext, TIdentityServerDbContext,
                    TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext,
                    TDataProtectionDbContext, TAuditLog>(options.ConnectionStrings, options.DatabaseProvider, options.DatabaseMigrations);
            }
            else
            {
                services.RegisterDbContextsStaging<TIdentityDbContext, TIdentityServerDbContext,
                    TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext,
                    TDataProtectionDbContext, TAuditLog>();
            }
            
            // Save data protection keys to db, using a common application name shared between Admin and STS
            services.AddDataProtection<TDataProtectionDbContext>(options.DataProtection, options.AzureKeyVault);

            // Add Asp.Net Core Identity Configuration and OpenIdConnect auth as well
            if (!options.Testing.IsStaging)
            {
                services.AddAuthenticationServices<TIdentityDbContext, TUser, TRole>
                        (options.Admin, options.IdentityConfigureAction, options.Security.AuthenticationBuilderAction);
            }
            else
            {
                services.AddAuthenticationServicesStaging<TIdentityDbContext, TUser, TRole>();
            }

            // Add HSTS options
            if (options.Security.UseHsts)
            {
                services.AddHsts(opt =>
                {
                    opt.Preload = true;
                    opt.IncludeSubDomains = true;
                    opt.MaxAge = TimeSpan.FromDays(365);

                    options.Security.HstsConfigureAction?.Invoke(opt);
                });
            }

            // Add exception filters in MVC
            services.AddMvcExceptionFilters();

            // Add all dependencies for IdentityServer Admin
            services.AddAdminServices<TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext>();

            // Add all dependencies for Asp.Net Core Identity
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext,
                TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim,
                TUserRole, TUserLogin, TRoleClaim, TUserToken, TUsersDto, TRolesDto, TUserRolesDto,
                TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto,
                TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>();

            // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
            // Including settings for MVC and Localization
            services.AddMvcWithLocalization<TUserDto, TRoleDto,
                TUser, TRole, TKey, TUserClaim, TUserRole,
                TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto,
                TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto,
                TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(options.Culture);

            // Add authorization policies for MVC
            services.AddAuthorizationPolicies(options.Admin, options.Security.AuthorizationConfigureAction);

            // Add audit logging
            services.AddAuditEventLogging<TAuditLogDbContext, TAuditLog>(options.AuditLogging);

            // Add health checks.
            var healthChecksBuilder = options.HealthChecksBuilderFactory?.Invoke(services) ?? services.AddHealthChecks();
            healthChecksBuilder.AddIdSHealthChecks<TIdentityServerDbContext, TPersistedGrantDbContext,
                TIdentityDbContext, TLogDbContext, TAuditLogDbContext,
                TDataProtectionDbContext, TAuditLog>(options.Admin, options.ConnectionStrings, options.DatabaseProvider);

            // Adds a startup filter for further middleware configuration.
            services.AddSingleton(options.Testing);
            services.AddSingleton(options.Security);
            services.AddSingleton(options.Http);
            services.AddTransient<IStartupFilter, StartupFilter>();

            return services;
        }
    }
}
