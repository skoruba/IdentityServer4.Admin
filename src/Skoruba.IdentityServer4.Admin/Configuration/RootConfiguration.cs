using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    internal class RootConfiguration
    {
        public AdminConfiguration AdminConfiguration { get; set; }

		public ConnectionStringsConfiguration ConnectionStrings { get; set; }

		internal RootConfiguration()
		{
			AdminConfiguration = new AdminConfiguration();
			ConnectionStrings = new ConnectionStringsConfiguration();
		}
    }
}
