using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.Helpers;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AdminServiceCollectionExtensions
	{
		//public static IServiceCollection AddIdentityServerAdminUI<TIdentityDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey>(this IServiceCollection services, IConfigurationRoot configuration, IHostingEnvironment env)
		//	where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
		//	where TUser : IdentityUser<TKey>
		//	where TRole : IdentityRole<TKey>
		//	where TUserClaim : IdentityUserClaim<TKey>
		//	where TUserRole : IdentityUserRole<TKey>
		//	where TUserLogin : IdentityUserLogin<TKey>
		//	where TRoleClaim : IdentityRoleClaim<TKey>
		//	where TUserToken : IdentityUserToken<TKey>
		//	where TKey : IEquatable<TKey>
		//{
		//	services.ConfigureRootConfiguration(configuration);

		//	services.AddDbContexts<TIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(builder.HostingEnvironment, builder.ConfigurationRoot, Assembly.GetCallingAssembly().GetName().Name);

		//	using (ServiceProvider serviceProvider = services.BuildServiceProvider())
		//	{
		//		IRootConfiguration rootConfiguration = serviceProvider.GetService<IRootConfiguration>();

		//		services.AddAuthenticationServices<TIdentityDbContext, TUser, TRole>(builder.HostingEnvironment, rootConfiguration.AdminConfiguration);
		//	}

		//	services.AddAuthorizationPolicies();

		//	services.AddMvcExceptionFilters();

		//	services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

		//	services.AddAdminAspNetIdentityServices<TIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<TKey>, TKey, RoleDto<TKey>, TKey, TKey,
		//		TKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
		//		UsersDto<UserDto<TKey>, TKey>, RolesDto<RoleDto<TKey>, TKey>, UserRolesDto<RoleDto<TKey>, TKey, TKey>,
		//		UserClaimsDto<TKey>, UserProviderDto<TKey>, UserProvidersDto<TKey>, UserChangePasswordDto<TKey>,
		//		RoleClaimsDto<TKey>, UserClaimDto<TKey>, RoleClaimDto<TKey>>();

		//	services.AddMvcWithLocalization<UserDto<TKey>, TKey, RoleDto<TKey>, TKey, TKey, TKey,
		//		TUser, TRole, TKey, TUserClaim, TUserRole,
		//		TUserLogin, TRoleClaim, TUserToken,
		//		UsersDto<UserDto<TKey>, TKey>, RolesDto<RoleDto<TKey>, TKey>, UserRolesDto<RoleDto<TKey>, TKey, TKey>,
		//		UserClaimsDto<TKey>, UserProviderDto<TKey>, UserProvidersDto<TKey>, UserChangePasswordDto<TKey>,
		//		RoleClaimsDto<TKey>>();

		//	return services;
		//}

		public static IServiceCollection AddIdentityServerAdminUI<TIdentityDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey>(this IServiceCollection services, IConfigurationRoot configuration, IHostingEnvironment env)
		where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TUserClaim : IdentityUserClaim<TKey>
		where TUserRole : IdentityUserRole<TKey>
		where TUserLogin : IdentityUserLogin<TKey>
		where TRoleClaim : IdentityRoleClaim<TKey>
		where TUserToken : IdentityUserToken<TKey>
		where TKey : IEquatable<TKey>
		{
			return AddIdentityServerAdminUI<TIdentityDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey>
				(services, options =>
			{
				options.ApplyConfiguration(configuration);
				options.IsStaging = env.IsStaging();
			});
		}

		public static IServiceCollection AddIdentityServerAdminUI<TIdentityDbContext, TUser>(this IServiceCollection services, Action<IdentityServerAdminOptions> optionsAction)
			where TIdentityDbContext : IdentityDbContext<TUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
			where TUser : IdentityUser<string>
			=> AddIdentityServerAdminUI<TIdentityDbContext, TUser, IdentityRole, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>, string>(services, optionsAction);

			public static IServiceCollection AddIdentityServerAdminUI<TIdentityDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey>(this IServiceCollection services, Action<IdentityServerAdminOptions> optionsAction)
			where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
			where TUser : IdentityUser<TKey>
			where TRole : IdentityRole<TKey>
			where TUserClaim : IdentityUserClaim<TKey>
			where TUserRole : IdentityUserRole<TKey>
			where TUserLogin : IdentityUserLogin<TKey>
			where TRoleClaim : IdentityRoleClaim<TKey>
			where TUserToken : IdentityUserToken<TKey>
			where TKey : IEquatable<TKey>
		{
			IdentityServerAdminOptions options = new IdentityServerAdminOptions(services);

			optionsAction(options);

			services.ConfigureRootConfiguration(options);

			services.AddDbContexts<TIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(options, Assembly.GetCallingAssembly().GetName().Name);

			//using (ServiceProvider serviceProvider = services.BuildServiceProvider())
			//{
				//IRootConfiguration rootConfiguration = serviceProvider.GetService<IRootConfiguration>();

				services.AddAuthenticationServices<TIdentityDbContext, TUser, TRole>(options);
			//}

			services.AddAuthorizationPolicies();

			services.AddMvcExceptionFilters();

			services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

			services.AddAdminAspNetIdentityServices<TIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<TKey>, TKey, RoleDto<TKey>, TKey, TKey,
				TKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
				UsersDto<UserDto<TKey>, TKey>, RolesDto<RoleDto<TKey>, TKey>, UserRolesDto<RoleDto<TKey>, TKey, TKey>,
				UserClaimsDto<TKey>, UserProviderDto<TKey>, UserProvidersDto<TKey>, UserChangePasswordDto<TKey>,
				RoleClaimsDto<TKey>, UserClaimDto<TKey>, RoleClaimDto<TKey>>();

			services.AddMvcWithLocalization<UserDto<TKey>, TKey, RoleDto<TKey>, TKey, TKey, TKey,
				TUser, TRole, TKey, TUserClaim, TUserRole,
				TUserLogin, TRoleClaim, TUserToken,
				UsersDto<UserDto<TKey>, TKey>, RolesDto<RoleDto<TKey>, TKey>, UserRolesDto<RoleDto<TKey>, TKey, TKey>,
				UserClaimsDto<TKey>, UserProviderDto<TKey>, UserProvidersDto<TKey>, UserChangePasswordDto<TKey>,
				RoleClaimsDto<TKey>>();

			return services;
		}
	}
}
