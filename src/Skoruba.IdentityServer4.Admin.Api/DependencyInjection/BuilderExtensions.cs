using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using IdentityServer4.EntityFramework.Options;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Storage;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using System.Collections.Generic;
using Skoruba.IdentityServer4.Admin.Api.Mappers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skoruba.IdentityServer4.Admin.Api.Helpers.Localization;
using Skoruba.IdentityServer4.Admin.Api.Configuration.ApplicationParts;
using Microsoft.AspNetCore.Mvc;

namespace Skoruba.IdentityServer4.Admin.Api.DependencyInjection
{
    public static partial class BuilderExtensions
    {
        public static IBuilder AddCustomConfiguration
            <TAdminIdentityDbContext, TIdentityServerConfigurationDbContext, TIdentityServerPersistedGrantDbContext, TAdminLogDbContext>
            (this IServiceCollection services,
            Action<DbContextOptionsBuilder> identityDbContextOptions,
            Action<ConfigurationStoreOptions> configurationStoreOptions,
            Action<OperationalStoreOptions> operationalStoreOptions,
            Action<DbContextOptionsBuilder> logDbContextOptions)
            where TAdminIdentityDbContext : DbContext
            where TIdentityServerConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TIdentityServerPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TAdminLogDbContext : DbContext, IAdminLogDbContext
        {
            var builder = new Builder(services);

            services.AddDbContext<TAdminIdentityDbContext>(identityDbContextOptions);
            services.AddConfigurationDbContext<TIdentityServerConfigurationDbContext>(configurationStoreOptions);
            services.AddOperationalDbContext<TIdentityServerPersistedGrantDbContext>(operationalStoreOptions);
            services.AddDbContext<TAdminLogDbContext>(logDbContextOptions);

            return builder;
        }

        public static IBuilder AddCustomIdentity
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

        private static HashSet<Type> ProfileTypes => new HashSet<Type>
            {
                typeof(IdentityMapperProfile<RoleDto<string>, string, UserRolesDto<RoleDto<string>, string, string>, string, UserClaimsDto<string>, UserClaimDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,RoleClaimDto<string>, RoleClaimsDto<string>>)
            };

        /// <summary>
        /// Register services for MVC
        /// </summary>
        /// <param name="services"></param>
        public static IBuilder AddMvcServices<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey,
            TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>
            (this IBuilder builder)
            where TUserDto : UserDto<TUserDtoKey>, new()
            where TRoleDto : RoleDto<TRoleDtoKey>, new()
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TRoleDtoKey : IEquatable<TRoleDtoKey>
            where TUserDtoKey : IEquatable<TUserDtoKey>
            where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
            where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
            where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
            where TUserProviderDto : UserProviderDto<TUserDtoKey>
            where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
            where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
        {
            builder.Services.TryAddTransient(typeof(IGenericControllerLocalizer<>), typeof(GenericControllerLocalizer<>));

            builder.Services.AddMvc(o =>
            {
                o.Conventions.Add(new GenericControllerRouteConvention());
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddDataAnnotationsLocalization()
                .ConfigureApplicationPartManager(m =>
                {
                    m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>());
                });

            return builder;
        }

        public static IBuilder AddUserValidator<TUser, TValidator>(this IBuilder builder)
            where TUser : class
            where TValidator : class
        {
            builder.Services.AddScoped(typeof(IUserValidator<TUser>), typeof(TValidator));
            return builder;
        }
    }
}