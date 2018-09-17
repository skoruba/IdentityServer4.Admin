using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Extensions
{
    public static class AdminServicesExtensions
    {
        private class MapperConfigurationBuilder : IMapperConfigurationBuilder
        {
            public HashSet<Type> ProfileTypes { get; } = new HashSet<Type>();

            public IMapperConfigurationBuilder UseIdentityMappingProfile<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUser, TRole, TKey,
                TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>() where TUserDto : UserDto<TUserDtoKey> where TRoleDto : RoleDto<TRoleDtoKey> where TUser : IdentityUser<TKey> where TRole : IdentityRole<TKey> where TKey : IEquatable<TKey> where TUserClaim : IdentityUserClaim<TKey> where TUserRole : IdentityUserRole<TKey> where TUserLogin : IdentityUserLogin<TKey> where TRoleClaim : IdentityRoleClaim<TKey> where TUserToken : IdentityUserToken<TKey>
            {
                ProfileTypes.Add(typeof(IdentityMapperProfile<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUser, TRole, TKey,
                    TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>));

                return this;
            }
        }

        public static IMapperConfigurationBuilder AddAdminMapping(this IServiceCollection services)
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

        public static IServiceCollection AddAdminServices<TAdminDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey,
            TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(
            this IServiceCollection services)
            where TAdminDbContext :
            IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>,
            IAdminPersistedGrantIdentityDbContext, IAdminConfigurationDbContext, IAdminLogDbContext
            where TUserDto : UserDto<TUserDtoKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TRoleDto : RoleDto<TRoleDtoKey>
        {

            return services.AddAdminServices<TAdminDbContext, TAdminDbContext, TAdminDbContext, TAdminDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey,
                TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>();
        }

        public static IServiceCollection AddAdminServices<TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TIdentityDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(this IServiceCollection services)
            where TPersistedGrantDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IAdminPersistedGrantIdentityDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
            where TUserDto : UserDto<TUserDtoKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TRoleDto : RoleDto<TRoleDtoKey>
            where TLogDbContext : DbContext, IAdminLogDbContext
        {
            //Repositories
            services.AddTransient<IClientRepository<TConfigurationDbContext>, ClientRepository<TConfigurationDbContext>>();
            services.AddTransient<IIdentityResourceRepository<TConfigurationDbContext>, IdentityResourceRepository<TConfigurationDbContext>>();
            services.AddTransient<IIdentityRepository<TIdentityDbContext, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IdentityRepository<TIdentityDbContext, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<IApiResourceRepository<TConfigurationDbContext>, ApiResourceRepository<TConfigurationDbContext>>();
            services.AddTransient<IPersistedGrantRepository<TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, PersistedGrantRepository<TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<ILogRepository<TLogDbContext>, LogRepository<TLogDbContext>>();

            //Services
            services.AddTransient<IClientService<TConfigurationDbContext>, ClientService<TConfigurationDbContext>>();
            services.AddTransient<IApiResourceService<TConfigurationDbContext>, ApiResourceService<TConfigurationDbContext>>();
            services.AddTransient<IIdentityResourceService<TConfigurationDbContext>, IdentityResourceService<TConfigurationDbContext>>();
            services.AddTransient<IIdentityService<TIdentityDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IdentityService<TIdentityDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<IPersistedGrantService<TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, PersistedGrantService<TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<ILogService<TLogDbContext>, LogService<TLogDbContext>>();

            //Resources
            services.AddScoped<IApiResourceServiceResources, ApiResourceServiceResources>();
            services.AddScoped<IClientServiceResources, ClientServiceResources>();
            services.AddScoped<IIdentityResourceServiceResources, IdentityResourceServiceResources>();
            services.AddScoped<IIdentityServiceResources, IdentityServiceResources>();
            services.AddScoped<IPersistedGrantServiceResources, PersistedGrantServiceResources>();

            //Register mapping
            services.AddAdminMapping()
                    .UseIdentityMappingProfile<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>();

            return services;
        }
    }
}
