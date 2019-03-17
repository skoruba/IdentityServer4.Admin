using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	public class IdentityServerAdminOptions
	{
		public bool IsStaging { get; set; }

		public ConnectionStringsConfiguration ConnectionStrings => RootConfiguration.ConnectionStrings;

		public AdminConfiguration AdminConfiguration => RootConfiguration.AdminConfiguration;

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
