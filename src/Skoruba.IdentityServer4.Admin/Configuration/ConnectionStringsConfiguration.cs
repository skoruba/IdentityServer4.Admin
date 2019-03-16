using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
	public class ConnectionStringsConfiguration
	{
		public string ConfigurationDbConnection { get; set; }

		public string PersistedGrantDbConnection { get; set; }

		public string AdminLogDbConnection { get; set; }

		public string IdentityDbConnection { get; set; }
	}
}
