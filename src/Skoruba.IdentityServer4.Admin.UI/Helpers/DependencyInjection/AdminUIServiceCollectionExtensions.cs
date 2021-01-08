using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.UI.Helpers;
using Skoruba.IdentityServer4.Shared.Dtos;
using Skoruba.IdentityServer4.Shared.Dtos.Identity;
using Skoruba.IdentityServer4.Shared.Helpers;
using System;

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
		public static IServiceCollection AddIdentityServer4AdminUI(this IServiceCollection services, Action<IdentityServer4AdminUIOptions> optionsAction)
			=> AddIdentityServer4AdminUI<AdminIdentityDbContext, UserIdentity, UserIdentityRole, UserIdentityUserClaim,
				UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken, string,
				IdentityUserDto, IdentityRoleDto, IdentityUsersDto, IdentityRolesDto, IdentityUserRolesDto,
				IdentityUserClaimsDto, IdentityUserProviderDto, IdentityUserProvidersDto, IdentityUserChangePasswordDto,
				IdentityRoleClaimsDto, IdentityUserClaimDto, IdentityRoleClaimDto>(services, optionsAction);

		/// <summary>
		/// Adds the Skoruba IdentityServer4 Admin UI with a custom user model and database context.
		/// </summary>
		/// <typeparam name="TIdentityDbContext"></typeparam>
		/// <typeparam name="TUser"></typeparam>
		/// <param name="services"></param>
		/// <param name="optionsAction"></param>
		/// <returns></returns>
		public static IServiceCollection AddIdentityServer4AdminUI<TIdentityDbContext, TUser>(this IServiceCollection services, Action<IdentityServer4AdminUIOptions> optionsAction)
			where TIdentityDbContext : IdentityDbContext<TUser, IdentityRole, string, IdentityUserClaim<string>,
				IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, 
				IdentityUserToken<string>>
			where TUser : IdentityUser<string>
			=> AddIdentityServer4AdminUI<TIdentityDbContext, TUser, IdentityRole, IdentityUserClaim<string>, 
				IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, 
				IdentityUserToken<string>, string, IdentityUserDto, IdentityRoleDto, IdentityUsersDto, IdentityRolesDto,
				IdentityUserRolesDto, IdentityUserClaimsDto, IdentityUserProviderDto, IdentityUserProvidersDto, 
				IdentityUserChangePasswordDto, IdentityRoleClaimsDto, IdentityUserClaimDto, IdentityRoleClaimDto>(services, optionsAction);

		/// <summary>
		/// Adds the Skoruba IdentityServer4 Admin UI with a fully custom entity model and database contexts.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="optionsAction"></param>
		/// <returns></returns>
		public static IServiceCollection AddIdentityServer4AdminUI<TIdentityDbContext, TUser, TRole, TUserClaim,
			TUserRole, TUserLogin, TRoleClaim, TUserToken, TKey, TUserDto, TRoleDto, TUsersDto, TRolesDto, TUserRolesDto,
			TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto,
			TRoleClaimDto>
			(this IServiceCollection services, Action<IdentityServer4AdminUIOptions> optionsAction)
			where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
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
		{
			// Builds the options from user preferences or configuration.
			IdentityServer4AdminUIOptions options = new IdentityServer4AdminUIOptions();
			optionsAction(options);
			services.AddSingleton(options);

			// Adds root configuration to the DI.
			services.AddSingleton(options.Admin);
			services.AddSingleton(options.IdentityServerData);
			services.AddSingleton(options.IdentityData);

			// Add DbContexts for Asp.Net Core Identity, Logging and IdentityServer - Configuration store and Operational store
			if (!options.IsStaging)
			{
				services.RegisterDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext,
					IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext,
					IdentityServerDataProtectionDbContext>(options.ConnectionStrings, options.DatabaseProvider);
			}
			else
			{
				services.RegisterDbContextsStaging<AdminIdentityDbContext, IdentityServerConfigurationDbContext,
					IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext,
					IdentityServerDataProtectionDbContext>();
			}


			// Save data protection keys to db, using a common application name shared between Admin and STS
			services.AddDataProtection<IdentityServerDataProtectionDbContext>(options.DataProtection, options.AzureKeyVault);

			// Add Asp.Net Core Identity Configuration and OpenIdConnect auth as well
			if (!options.IsStaging)
			{
				services.AddAuthenticationServices<AdminIdentityDbContext, UserIdentity, UserIdentityRole>
						(options.Admin, options.IdentityAction);
			}
			else
			{
				services.AddAuthenticationServicesStaging<AdminIdentityDbContext, UserIdentity, UserIdentityRole>();
			}

			// Add HSTS options
			services.AddHsts(options =>
			{
				options.Preload = true;
				options.IncludeSubDomains = true;
				options.MaxAge = TimeSpan.FromDays(365);
			});

			// Add exception filters in MVC
			services.AddMvcExceptionFilters();

			// Add all dependencies for IdentityServer Admin
			services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

			// Add all dependencies for Asp.Net Core Identity
			// If you want to change primary keys or use another db model for Asp.Net Core Identity:
			services.AddAdminAspNetIdentityServices<TIdentityDbContext, IdentityServerPersistedGrantDbContext,
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
			services.AddAuthorizationPolicies(options.Admin);

			// Add audit logging
			services.AddAuditEventLogging<AdminAuditLogDbContext, AuditLog>(options.AuditLogging);

			// Add health checks.
			services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext,
				TIdentityDbContext, AdminLogDbContext, AdminAuditLogDbContext,
				IdentityServerDataProtectionDbContext>(options.Admin, options.ConnectionStrings, options.DatabaseProvider);

			return services;
		}
	}
}
