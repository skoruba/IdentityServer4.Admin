namespace Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.Configuration
{
    public class DatabaseMigrationsConfiguration
    {
        public bool ApplyDatabaseMigrations { get; set; } = false;

		public string ConfigurationDbMigrationsAssembly { get; set; }

		public string PersistedGrantDbMigrationsAssembly { get; set; }

		public string AdminLogDbMigrationsAssembly { get; set; }

		public string IdentityDbMigrationsAssembly { get; set; }

		public string AdminAuditLogDbMigrationsAssembly { get; set; }

		public string DataProtectionDbMigrationsAssembly { get; set; }

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