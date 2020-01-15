using System;
using System.Collections.Generic;
using AutoMapper;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AdminServicesExtensions
    {
        public static IServiceCollection AddAdminAspNetIdentityServices(this IServiceCollection services) =>
            AddAdminAspNetIdentityServices<string>(services, null, null);

        public static IServiceCollection AddAdminAspNetIdentityServices(this IServiceCollection services, HashSet<Type> profileTypes) =>
            AddAdminAspNetIdentityServices<string>(services, null, profileTypes);

        public static IServiceCollection AddAdminAspNetIdentityServices(this IServiceCollection services, Func<IAdminAspNetIdentityConfigurationBuilder<string>, IConfiguredAdminAspNetIdentityConfigurationBuilder<string>> configure) =>
            AddAdminAspNetIdentityServices<string>(services, configure, null);

        public static IServiceCollection AddAdminAspNetIdentityServices<TKey>(this IServiceCollection services, Func<IAdminAspNetIdentityConfigurationBuilder<TKey>, IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey>> configure = null, HashSet<Type> profileTypes = null)
            where TKey : IEquatable<TKey>
        {
            if (configure is null)
                throw new NotImplementedException("We have no default IdentityDbContext and IdentityServerPersistedGrantDbContext in here");
            var builder = new AdminAspNetIdentityConfigurationBuilder<TKey>();
            var configuration = configure(builder).Build();
            services.AddRepositories(configuration);
            services.AddServices(configuration);
            AddResources();
            services.AddMapping(configuration, profileTypes);
            return services;

            void AddResources()
            {
                services.AddScoped<IIdentityServiceResources, IdentityServiceResources>();
                services.AddScoped<IPersistedGrantAspNetIdentityServiceResources, PersistedGrantAspNetIdentityServiceResources>();
            }
        }

        private static void AddRepositories<TKey>(this IServiceCollection services, AdminAspNetIdentityConfiguration<TKey> configuration)
            where TKey : IEquatable<TKey>
        {
            AddIdentityRepository();
            AddPersistedGrantAspNetIdentityRepository();

            void AddIdentityRepository()
            {
                var genericInterface = typeof(IIdentityRepository<,,,,,,,>);
                Type[] interfaceTypes = {
                    configuration.UserType,
                    configuration.RoleType,
                    typeof(TKey),
                    configuration.UserClaimType,
                    configuration.UserRoleType,
                    configuration.UserLoginType,
                    configuration.RoleClaimType,
                    configuration.UserTokenType
                };
                var constructedInterface = genericInterface.MakeGenericType(interfaceTypes);
                var genericImplementation = typeof(IdentityRepository<,,,,,,,,>);
                Type[] implementationTypes = {
                    configuration.IdentityDbContextType,
                    configuration.UserType,
                    configuration.RoleType,
                    typeof(TKey),
                    configuration.UserClaimType,
                    configuration.UserRoleType,
                    configuration.UserLoginType,
                    configuration.RoleClaimType,
                    configuration.UserTokenType
                };
                var constructedImplementation = genericImplementation.MakeGenericType(implementationTypes);
                services.AddTransient(constructedInterface, constructedImplementation);
            }

            void AddPersistedGrantAspNetIdentityRepository()
            {
                var @interface = typeof(IPersistedGrantAspNetIdentityRepository);
                var genericImplementation = typeof(PersistedGrantAspNetIdentityRepository<,,,,,,,,,>);
                Type[] implementationTypes = {
                    configuration.IdentityDbContextType,
                    configuration.PersistedGrantDbContextType,
                    configuration.UserType,
                    configuration.RoleType,
                    typeof(TKey),
                    configuration.UserClaimType,
                    configuration.UserRoleType,
                    configuration.UserLoginType,
                    configuration.RoleClaimType,
                    configuration.UserTokenType
                };
                var constructedImplementation = genericImplementation.MakeGenericType(implementationTypes);
                services.AddTransient(@interface, constructedImplementation);
            }
        }

        private static void AddServices<TKey>(this IServiceCollection services, AdminAspNetIdentityConfiguration<TKey> configuration)
            where TKey : IEquatable<TKey>
        {
            AddIdentityService();
            AddPersistedGrandService();

            void AddIdentityService()
            {
                var genericInterface = typeof(IIdentityService<,,,,,,,,,,,,,,,,,>);
                Type[] interfaceTypes = {
                    configuration.DtoTypes.User,
                    configuration.DtoTypes.Role,
                    configuration.UserType,
                    configuration.RoleType,
                    typeof(TKey),
                    configuration.UserClaimType,
                    configuration.UserRoleType,
                    configuration.UserLoginType,
                    configuration.RoleClaimType,
                    configuration.UserTokenType,
                    configuration.DtoTypes.Users,
                    configuration.DtoTypes.Roles,
                    configuration.DtoTypes.UserRoles,
                    configuration.DtoTypes.UserClaims,
                    configuration.DtoTypes.UserProvider,
                    configuration.DtoTypes.UserProviders,
                    configuration.DtoTypes.UserChangePassword,
                    configuration.DtoTypes.RoleClaims,
                };
                var constructedInterface = genericInterface.MakeGenericType(interfaceTypes);
                var genericImplementation = typeof(IdentityService<,,,,,,,,,,,,,,,,,>);
                Type[] implementationTypes = {
                    configuration.DtoTypes.User,
                    configuration.DtoTypes.Role,
                    configuration.UserType,
                    configuration.RoleType,
                    typeof(TKey),
                    configuration.UserClaimType,
                    configuration.UserRoleType,
                    configuration.UserLoginType,
                    configuration.RoleClaimType,
                    configuration.UserTokenType,
                    configuration.DtoTypes.Users,
                    configuration.DtoTypes.Roles,
                    configuration.DtoTypes.UserRoles,
                    configuration.DtoTypes.UserClaims,
                    configuration.DtoTypes.UserProvider,
                    configuration.DtoTypes.UserProviders,
                    configuration.DtoTypes.UserChangePassword,
                    configuration.DtoTypes.RoleClaims,
                };
                var constructedImplementation = genericImplementation.MakeGenericType(implementationTypes);
                services.AddTransient(constructedInterface, constructedImplementation);
            }

            void AddPersistedGrandService()
            {
                services.AddTransient<IPersistedGrantAspNetIdentityService, PersistedGrantAspNetIdentityService>();
            }
        }

        private static void AddMapping<TKey>(this IServiceCollection services, AdminAspNetIdentityConfiguration<TKey> configuration, HashSet<Type> profileTypes)
            where TKey : IEquatable<TKey>
        {
            var builder = AddAdminAspNetIdentityMapping();
            var method = builder.GetType().GetMethod(nameof(builder.UseIdentityMappingProfile));
            Type[] methodTypes = {
                    configuration.DtoTypes.User,
                    configuration.DtoTypes.Role,
                    configuration.UserType,
                    configuration.RoleType,
                    typeof(TKey),
                    configuration.UserClaimType,
                    configuration.UserRoleType,
                    configuration.UserLoginType,
                    configuration.RoleClaimType,
                    configuration.UserTokenType,
                    configuration.DtoTypes.Users,
                    configuration.DtoTypes.Roles,
                    configuration.DtoTypes.UserRoles,
                    configuration.DtoTypes.UserClaims,
                    configuration.DtoTypes.UserProvider,
                    configuration.DtoTypes.UserProviders,
                    configuration.DtoTypes.UserChangePassword,
                    configuration.DtoTypes.RoleClaims,
                    configuration.DtoTypes.UserClaim,
                    configuration.DtoTypes.RoleClaim
                };
            var genericMethod = method.MakeGenericMethod(methodTypes);
            genericMethod.Invoke(builder, null);
            builder.AddProfilesType(profileTypes);

            IMapperConfigurationBuilder AddAdminAspNetIdentityMapping()
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
        }
    }
}
