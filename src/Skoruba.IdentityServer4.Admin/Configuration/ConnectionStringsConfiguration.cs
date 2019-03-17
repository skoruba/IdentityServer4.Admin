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
	}
}
