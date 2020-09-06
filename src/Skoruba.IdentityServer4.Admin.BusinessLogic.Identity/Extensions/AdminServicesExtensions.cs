using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AdminServicesExtensions
    {
        public static IMapperConfigurationBuilder AddAdminAspNetIdentityMapping(this IServiceCollection services)
        {
            var builder = new MapperConfigurationBuilder();

            services.AddSingleton<IConfigurationProvider>(sp => new MapperConfiguration(cfg =>
            {
                foreach (var profileType in builder.ProfileTypes)
                    cfg.AddProfile(profileType);
            }));

            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));

            return builder;
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUser>(
            this IServiceCollection services)
            where TIdentityDbContext : IdentityDbContext<TUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TUser : IdentityUser
        {
            return services.AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, UserDto<string>, RoleDto<string>,
                TUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>();
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TAdminDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey,
            TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto,
            TUserClaimDto, TRoleClaimDto>(
                this IServiceCollection services)
            where TAdminDbContext :
            IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>,
            IAdminPersistedGrantDbContext
            where TUserDto : UserDto<TKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TRoleDto : RoleDto<TKey>
            where TUsersDto : UsersDto<TUserDto, TKey>
            where TRolesDto : RolesDto<TRoleDto, TKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
            where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
            where TUserProviderDto : UserProviderDto<TKey>
            where TUserProvidersDto : UserProvidersDto<TKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TKey>
            where TRoleClaimsDto : RoleClaimsDto<TKey>
            where TUserClaimDto : UserClaimDto<TKey>
            where TRoleClaimDto : RoleClaimDto<TKey>
        {

            return services.AddAdminAspNetIdentityServices<TAdminDbContext, TAdminDbContext, TUserDto, TRoleDto,
                TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>();
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(
                        this IServiceCollection services)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
            where TUserDto : UserDto<TKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TRoleDto : RoleDto<TKey>
            where TUsersDto : UsersDto<TUserDto, TKey>
            where TRolesDto : RolesDto<TRoleDto, TKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
            where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
            where TUserProviderDto : UserProviderDto<TKey>
            where TUserProvidersDto : UserProvidersDto<TKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TKey>
            where TRoleClaimsDto : RoleClaimsDto<TKey>
            where TUserClaimDto : UserClaimDto<TKey>
            where TRoleClaimDto : RoleClaimDto<TKey>
        {
            return AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto,
                TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin,
                TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto,
                TRoleClaimDto>(services, null);
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(
                        this IServiceCollection services, HashSet<Type> profileTypes)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
            where TUserDto : UserDto<TKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TRoleDto : RoleDto<TKey>
            where TUsersDto : UsersDto<TUserDto, TKey>
            where TRolesDto : RolesDto<TRoleDto, TKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
            where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
            where TUserProviderDto : UserProviderDto<TKey>
            where TUserProvidersDto : UserProvidersDto<TKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TKey>
            where TRoleClaimsDto : RoleClaimsDto<TKey>
            where TUserClaimDto : UserClaimDto<TKey>
            where TRoleClaimDto : RoleClaimDto<TKey>
        {
            //Repositories
            services.AddTransient<IIdentityRepository<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IdentityRepository<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<IPersistedGrantAspNetIdentityRepository, PersistedGrantAspNetIdentityRepository<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
          
            //Services
            services.AddTransient<IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto>, 
                IdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto>>();
            services.AddTransient<IPersistedGrantAspNetIdentityService, PersistedGrantAspNetIdentityService>();
            
            //Resources
            services.AddScoped<IIdentityServiceResources, IdentityServiceResources>();
            services.AddScoped<IPersistedGrantAspNetIdentityServiceResources, PersistedGrantAspNetIdentityServiceResources>();

            //Register mapping
            services.AddAdminAspNetIdentityMapping()
                .UseIdentityMappingProfile<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim,
                    TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto,
                    TRoleClaimDto>()
                .AddProfilesType(profileTypes);

            return services;
        }
    }
}
