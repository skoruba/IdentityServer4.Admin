using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
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
		//public static IIdentityServerAdminBuilder AddIdentityServerAdminUI(this IServiceCollection services)
		//{
		//	var builder = services
		//			.AddIdentityServerAdminUIBuilder();

		//	//builder.Services.AddMvcExceptionFilters();
		//	//builder.Services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();
		//	//builder.Services.AddAuthorizationPolicies();

		//	return builder;

		//	//var rootConfiguration = services.BuildServiceProvider().GetService<IRootConfiguration>();

		//	//services.AddDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(HostingEnvironment, Configuration);

		//	services.AddAuthenticationServices<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(HostingEnvironment, rootConfiguration.AdminConfiguration);

		//	//services.AddMvcExceptionFilters();

		//	//services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

		//	services.AddAdminAspNetIdentityServices<AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<string>, string, RoleDto<string>, string, string, string,
		//						UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
		//						UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
		//						UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
		//						UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
		//						RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>();

		//	services.AddMvcWithLocalization<UserDto<string>, string, RoleDto<string>, string, string, string,
		//		UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
		//		UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
		//		UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
		//		UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
		//		RoleClaimsDto<string>>();

		//	//services.AddAuthorizationPolicies();
		//}

		public static IIdentityServerAdminBuilder AddIdentityServerAdminUI(this IServiceCollection services, IConfigurationRoot configuration, IHostingEnvironment env)
		{
			services.ConfigureRootConfiguration(configuration);
			
			var builder = services.AddIdentityServerAdminUIBuilder(env, configuration);

			builder.Services.AddMvcExceptionFilters();

			builder.Services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

			return builder;
		}

		private static IIdentityServerAdminBuilder AddIdentityServerAdminUIBuilder(this IServiceCollection services, IHostingEnvironment env, IConfigurationRoot configurationRoot)
		{
			return new IdentityServerAdminBuilder(services, env, configurationRoot);
		}

		#region Builder Extensions

		//public static IIdentityServerAdminBuilder AddIdentityServerInMemoryStores<TIdentityDbContext>(this IIdentityServerAdminBuilder builder)
		//	where TIdentityDbContext : DbContext
		//{
		//	return builder.RegisterDbContextsStaging<TIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();
		//}

		//public static IIdentityServerAdminBuilder AddIdentityServerStores<TIdentityDbContext>(this IIdentityServerAdminBuilder builder, IConfigurationRoot configurationRoot)
		//	where TIdentityDbContext : DbContext
		//{
		//	return builder.RegisterDbContexts<TIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(configurationRoot, Assembly.GetCallingAssembly().GetName().Name);
		//}

		public static IIdentityServerAdminBuilder AddIdentityServerStores<TIdentityDbContext>(this IIdentityServerAdminBuilder builder)
			where TIdentityDbContext : DbContext
		{
			//builder.Services.RegisterDbContexts<TIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(builder.ConfigurationRoot, Assembly.GetCallingAssembly().GetName().Name);
			builder.Services.AddDbContexts<TIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(builder.HostingEnvironment, builder.ConfigurationRoot, Assembly.GetCallingAssembly().GetName().Name);

			return builder;
		}

		public static IIdentityServerAdminBuilder AddAuthentication<TIdentityDbContext, TUserIdentity, TUserIdentityRole>(this IIdentityServerAdminBuilder builder)
			where TIdentityDbContext : DbContext 
			where TUserIdentity : class 
			where TUserIdentityRole : class
		{
			using (ServiceProvider serviceProvider = builder.Services.BuildServiceProvider())
			{
				IRootConfiguration rootConfiguration = serviceProvider.GetService<IRootConfiguration>();

				builder.Services.AddAuthenticationServices<TIdentityDbContext, TUserIdentity, TUserIdentityRole>(builder.HostingEnvironment, rootConfiguration.AdminConfiguration);

				builder.Services.AddAuthorizationPolicies();

				return builder;
			}
		}

		public static IIdentityServerAdminBuilder AddAspNetIdentity<TIdentityDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey>(this IIdentityServerAdminBuilder builder)
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
			builder.Services.AddAdminAspNetIdentityServices<TIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<TKey>, TKey, RoleDto<TKey>, TKey, TKey,
				TKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
				UsersDto<UserDto<TKey>, TKey>, RolesDto<RoleDto<TKey>, TKey>, UserRolesDto<RoleDto<TKey>, TKey, TKey>,
				UserClaimsDto<TKey>, UserProviderDto<TKey>, UserProvidersDto<TKey>, UserChangePasswordDto<TKey>,
				RoleClaimsDto<TKey>, UserClaimDto<TKey>, RoleClaimDto<TKey>>();

			builder.Services.AddMvcWithLocalization<UserDto<TKey>, TKey, RoleDto<TKey>, TKey, TKey, TKey,
				TUser, TRole, TKey, TUserClaim, TUserRole,
				TUserLogin, TRoleClaim, TUserToken,
				UsersDto<UserDto<TKey>, TKey>, RolesDto<RoleDto<TKey>, TKey>, UserRolesDto<RoleDto<TKey>, TKey, TKey>,
				UserClaimsDto<TKey>, UserProviderDto<TKey>, UserProvidersDto<TKey>, UserChangePasswordDto<TKey>,
				RoleClaimsDto<TKey>>();

			return builder;
		}

		#endregion
	}
}
