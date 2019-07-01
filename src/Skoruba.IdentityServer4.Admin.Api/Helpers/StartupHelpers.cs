using System;
using System.Reflection;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skoruba.IdentityServer4.Admin.Api.Configuration;
using Skoruba.IdentityServer4.Admin.Api.Configuration.ApplicationParts;
using Skoruba.IdentityServer4.Admin.Api.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Api.Helpers.Localization;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Api.Helpers
{
    public static class StartupHelpers
    {
        public static Action<DbContextOptionsBuilder> DefaultIdentityDbContextOptions(IConfiguration c) =>
            o => o.UseSqlServer(c.GetConnectionString(ConfigurationConsts.IdentityDbConnectionStringKey),
                sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

        public static Action<ConfigurationStoreOptions> DefaultIdentityServerConfigurationOptions(IConfiguration c) =>
            o => o.ConfigureDbContext = b =>
                   b.UseSqlServer(c.GetConnectionString(ConfigurationConsts.ConfigurationDbConnectionStringKey),
                        sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

        public static Action<OperationalStoreOptions> DefaultIdentityServerOperationalStoreOptions(IConfiguration c) =>
            o => o.ConfigureDbContext = b =>
                b.UseSqlServer(c.GetConnectionString(ConfigurationConsts.PersistedGrantDbConnectionStringKey),
                    sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

        public static Action<DbContextOptionsBuilder> DefaultLogDbContextOptions(IConfiguration c) =>
            o => o.UseSqlServer(c.GetConnectionString(ConfigurationConsts.AdminLogDbConnectionStringKey),
                sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

        public static Action<IdentityOptions> DefaultIdentityOptions(IConfiguration c) =>
            o => o.User.RequireUniqueEmail = true;

        ///// <summary>
        ///// Register services for MVC
        ///// </summary>
        ///// <param name="services"></param>
        //public static void AddMvcServices<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey,
        //    TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        //    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        //    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>(
        //    this IServiceCollection services)
        //    where TUserDto : UserDto<TUserDtoKey>, new()
        //    where TRoleDto : RoleDto<TRoleDtoKey>, new()
        //    where TUser : IdentityUser<TKey>
        //    where TRole : IdentityRole<TKey>
        //    where TKey : IEquatable<TKey>
        //    where TUserClaim : IdentityUserClaim<TKey>
        //    where TUserRole : IdentityUserRole<TKey>
        //    where TUserLogin : IdentityUserLogin<TKey>
        //    where TRoleClaim : IdentityRoleClaim<TKey>
        //    where TUserToken : IdentityUserToken<TKey>
        //    where TRoleDtoKey : IEquatable<TRoleDtoKey>
        //    where TUserDtoKey : IEquatable<TUserDtoKey>
        //    where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
        //    where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
        //    where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
        //    where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
        //    where TUserProviderDto : UserProviderDto<TUserDtoKey>
        //    where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
        //    where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
        //    where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
        //{
        //    services.TryAddTransient(typeof(IGenericControllerLocalizer<>), typeof(GenericControllerLocalizer<>));

        //    services.AddMvc(o =>
        //        {
        //            o.Conventions.Add(new GenericControllerRouteConvention());
        //        })
        //        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
        //        .AddDataAnnotationsLocalization()
        //        .ConfigureApplicationPartManager(m =>
        //        {
        //            m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        //                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        //                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>());
        //        });
        //}

        /// <summary>
        /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// Configure the connection strings in AppSettings.json
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TLogDbContext"></typeparam>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(this IServiceCollection services, IConfiguration configuration)
        where TIdentityDbContext : DbContext
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TLogDbContext : DbContext, IAdminLogDbContext
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Config DB for identity
            services.AddDbContext<TIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.IdentityDbConnectionStringKey),
                    sql => sql.MigrationsAssembly(migrationsAssembly)));

            // Config DB from existing connection
            services.AddConfigurationDbContext<TConfigurationDbContext>(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.ConfigurationDbConnectionStringKey),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            // Operational DB from existing connection
            services.AddOperationalDbContext<TPersistedGrantDbContext>(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.PersistedGrantDbConnectionStringKey),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            // Log DB from existing connection
            services.AddDbContext<TLogDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(ConfigurationConsts.AdminLogDbConnectionStringKey),
                    optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly)));
        }

        /// <summary>
        /// Add authentication middleware for an API
        /// </summary>
        public static void AddApiAuthentication(this IServiceCollection services, AdminApiConfiguration adminApiConfiguration)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = adminApiConfiguration.IdentityServerBaseUrl;
                    options.ApiName = adminApiConfiguration.OidcApiName;
#if DEBUG
                    options.RequireHttpsMetadata = false;
#else
                    options.RequireHttpsMetadata = true;
#endif
                });
        }

        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                    policy => policy.RequireRole(AuthorizationConsts.AdministrationRole));
            });
        }
    }
}