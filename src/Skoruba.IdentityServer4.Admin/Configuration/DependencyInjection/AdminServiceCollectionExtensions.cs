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
