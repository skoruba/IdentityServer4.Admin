using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Skoruba.IdentityServer4.Admin.Configuration;
using Skoruba.IdentityServer4.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Options for the Skoruba IdentityServer Admin UI.
	/// </summary>
	public class IdentityServerAdminOptions
	{
		/// <summary>
		/// Use test instead of production services and pipelines.
		/// </summary>
		public bool IsStaging { get; set; }

		/// <summary>
		/// Use the developer exception page instead of the Identity error handler.
		/// </summary>
		public bool UseDeveloperExceptionPage { get; set; }

		/// <summary>
		/// The database connection strings and settings.
		/// </summary>
		public ConnectionStringsConfiguration ConnectionStrings => RootConfiguration.ConnectionStrings;

		/// <summary>
		/// The settings for the admin services.
		/// </summary>
		public AdminConfiguration AdminConfiguration => RootConfiguration.AdminConfiguration;

		/// <summary>
		/// A custom action executed while configuring logging. Allows to add Serilog sinks.
		/// </summary>
		public Action<LoggerConfiguration> SerilogConfigurationBuilder { get; set; }

		/// <summary>
		/// Base filename for the logs. Leave null to disable file logging.
		/// </summary>
		public string LogFile { get; set; } = "Log\\skoruba_admin.txt";

		internal RootConfiguration RootConfiguration { get; }

		private readonly IServiceCollection services;

		internal IdentityServerAdminOptions(IServiceCollection services)
		{
			this.services = services;
			this.RootConfiguration = new RootConfiguration();
		}

		public void SetConnections(string adminConnectionString)
			=> SetConnections(adminConnectionString, Assembly.GetCallingAssembly().GetName().Name);

		public void SetConnections(string adminConnectionString, string migrationsAssembly)
			=> SetConnections(adminConnectionString, migrationsAssembly,
				adminConnectionString, migrationsAssembly,
				adminConnectionString, migrationsAssembly,
				adminConnectionString, migrationsAssembly);

		public void SetConnections(string configurationConnectionString, string persistedGrantConnectionString, string identityConnectionString, string adminLogConnectionString)
			=> SetConnections(configurationConnectionString, persistedGrantConnectionString, identityConnectionString, adminLogConnectionString, Assembly.GetCallingAssembly().GetName().Name);

		public void SetConnections(string configurationConnectionString, string persistedGrantConnectionString, string identityConnectionString, string adminLogConnectionString, string migrationsAssembly) 
			=> SetConnections(
				configurationConnectionString, migrationsAssembly,
				persistedGrantConnectionString, migrationsAssembly,
				identityConnectionString, migrationsAssembly,
				adminLogConnectionString, migrationsAssembly);

		public void SetConnections(
			string configurationConnectionString, 
			string configurationMigrationsAssembly,
			string persistedGrantConnectionString, 
			string persistedGrantMigrationsAssembly,
			string identityConnectionString, 
			string identityMigrationsAssembly,
			string adminLogConnectionString,
			string adminLogMigrationsAssembly)
		{
			RootConfiguration.ConnectionStrings.ConfigurationDbConnection = configurationConnectionString;
			RootConfiguration.ConnectionStrings.PersistedGrantDbConnection = persistedGrantConnectionString;
			RootConfiguration.ConnectionStrings.IdentityDbConnection = identityConnectionString;
			RootConfiguration.ConnectionStrings.AdminLogDbConnection = adminLogConnectionString;

			SetMigrationsAssemblies(configurationMigrationsAssembly, persistedGrantMigrationsAssembly, identityMigrationsAssembly, adminLogMigrationsAssembly);
		}

		public void SetMigrationsAssemblies(string migrationsAssembly)
			=> SetMigrationsAssemblies(migrationsAssembly, migrationsAssembly, migrationsAssembly, migrationsAssembly);

		public void SetMigrationsAssemblies(
			string configurationMigrationsAssembly,
			string persistedGrantMigrationsAssembly,
			string identityMigrationsAssembly,
			string adminLogMigrationsAssembly)
		{
			RootConfiguration.ConnectionStrings.ConfigurationDbMigrationsAssembly = configurationMigrationsAssembly;
			RootConfiguration.ConnectionStrings.PersistedGrantDbMigrationsAssembly = persistedGrantMigrationsAssembly;
			RootConfiguration.ConnectionStrings.IdentityDbMigrationsAssembly = identityMigrationsAssembly;
			RootConfiguration.ConnectionStrings.AdminLogDbMigrationsAssembly = adminLogMigrationsAssembly;
		}

		public void ApplyConfiguration(IConfigurationRoot configurationRoot)
		{
			configurationRoot.Bind(RootConfiguration);
		}
	}
}
