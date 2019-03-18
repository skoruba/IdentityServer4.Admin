using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
	public class ConnectionStringsConfiguration
	{
		public string ConfigurationDbConnection { get; set; }

		public string ConfigurationDbMigrationsAssembly { get; set; }

		public string PersistedGrantDbConnection { get; set; }

		public string PersistedGrantDbMigrationsAssembly { get; set; }

		public string AdminLogDbConnection { get; set; }

		public string AdminLogDbMigrationsAssembly { get; set; }

		public string IdentityDbConnection { get; set; }

		public string IdentityDbMigrationsAssembly { get; set; }

		public void SetConnections(string commonConnectionString, string commonMigrationsAssembly)
			=> SetConnections(commonConnectionString, commonConnectionString,
				commonConnectionString, commonConnectionString,
				commonMigrationsAssembly, commonMigrationsAssembly,
				commonMigrationsAssembly, commonMigrationsAssembly);

		public void SetConnections(
			string configurationConnectionString, 
			string persistedGrantConnectionString, 
			string identityConnectionString, 
			string adminLogConnectionString, 
			string commonMigrationsAssembly)
			=> SetConnections(
				configurationConnectionString, persistedGrantConnectionString,
				identityConnectionString, adminLogConnectionString,
				commonMigrationsAssembly, commonMigrationsAssembly,
				commonMigrationsAssembly, commonMigrationsAssembly);

		public void SetConnections(
			string configurationConnectionString,
			string persistedGrantConnectionString,
			string identityConnectionString,
			string adminLogConnectionString,
			string configurationMigrationsAssembly,
			string persistedGrantMigrationsAssembly,
			string identityMigrationsAssembly,
			string adminLogMigrationsAssembly)
		{
			ConfigurationDbConnection = configurationConnectionString;
			PersistedGrantDbConnection = persistedGrantConnectionString;
			IdentityDbConnection = identityConnectionString;
			AdminLogDbConnection = adminLogConnectionString;

			SetMigrationsAssemblies(configurationMigrationsAssembly, persistedGrantMigrationsAssembly, identityMigrationsAssembly, adminLogMigrationsAssembly);
		}

		public void SetMigrationsAssemblies(string commonMigrationsAssembly)
			=> SetMigrationsAssemblies(commonMigrationsAssembly, commonMigrationsAssembly, commonMigrationsAssembly, commonMigrationsAssembly);

		public void SetMigrationsAssemblies(
			string identityServerMigrationsAssembly,
			string adminLogMigrationsAssembly)
		{
			ConfigurationDbMigrationsAssembly = identityServerMigrationsAssembly;
			PersistedGrantDbMigrationsAssembly = identityServerMigrationsAssembly;
			IdentityDbMigrationsAssembly = identityServerMigrationsAssembly;
			AdminLogDbMigrationsAssembly = adminLogMigrationsAssembly;
		}

		public void SetMigrationsAssemblies(
			string configurationMigrationsAssembly,
			string persistedGrantMigrationsAssembly,
			string identityMigrationsAssembly,
			string adminLogMigrationsAssembly)
		{
			ConfigurationDbMigrationsAssembly = configurationMigrationsAssembly;
			PersistedGrantDbMigrationsAssembly = persistedGrantMigrationsAssembly;
			IdentityDbMigrationsAssembly = identityMigrationsAssembly;
			AdminLogDbMigrationsAssembly = adminLogMigrationsAssembly;
		}

	}
}
