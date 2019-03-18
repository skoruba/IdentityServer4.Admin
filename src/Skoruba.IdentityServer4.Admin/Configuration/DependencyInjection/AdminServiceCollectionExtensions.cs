using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Helpers;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AdminServiceCollectionExtensions
	{
		public static IServiceCollection AddIdentityServerAdminUI(this IServiceCollection services, IConfigurationRoot configuration, IHostingEnvironment env)
		{
			string callingAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
			return AddIdentityServerAdminUI<AdminIdentityDbContext, UserIdentity, UserIdentityRole, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken, string>(services, configuration, env, callingAssemblyName);
		}

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
			string callingAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
			return AddIdentityServerAdminUI<TIdentityDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey>(services, configuration, env, callingAssemblyName);
		}

		private static IServiceCollection AddIdentityServerAdminUI<TIdentityDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey>(this IServiceCollection services, IConfigurationRoot configuration, IHostingEnvironment env, string callingAssemblyName)
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
					options.SetMigrationsAssemblies(callingAssemblyName);
					options.IsStaging = env.IsStaging();
					options.UseDeveloperExceptionPage = env.IsDevelopment();
					options.SerilogConfigurationBuilder = serilog => serilog.ReadFrom.Configuration(configuration);
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

			services.AddDbContexts<TIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(options);

			services.AddAuthenticationServices<TIdentityDbContext, TUser, TRole>(options);
			
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
