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

		public string AdminAuditLogDbConnection { get; set; }

		public string AdminAuditLogDbMigrationsAssembly { get; set; }

		public string DataProtectionDbConnection { get; set; }

		public string DataProtectionDbMigrationsAssembly { get; set; }

		public void SetConnections(string commonConnectionString)
		{
			AdminAuditLogDbConnection = commonConnectionString;
			AdminLogDbConnection = commonConnectionString;
			ConfigurationDbConnection = commonConnectionString;
			DataProtectionDbConnection = commonConnectionString;
			IdentityDbConnection = commonConnectionString;
			PersistedGrantDbConnection = commonConnectionString;
		}

		public void SetMigrationsAssemblies(string commonMigrationsAssembly)
		{
			AdminAuditLogDbMigrationsAssembly = commonMigrationsAssembly;
			AdminLogDbMigrationsAssembly = commonMigrationsAssembly;
			ConfigurationDbMigrationsAssembly = commonMigrationsAssembly;
			DataProtectionDbMigrationsAssembly = commonMigrationsAssembly;
			IdentityDbMigrationsAssembly = commonMigrationsAssembly;
			PersistedGrantDbMigrationsAssembly = commonMigrationsAssembly;
		}
	}
}
