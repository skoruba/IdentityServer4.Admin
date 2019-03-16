using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    internal class RootConfiguration : IRootConfiguration
    {
        public IAdminConfiguration AdminConfiguration { get; set; }

		public ConnectionStringsConfiguration ConnectionStrings { get; set; }

		//internal RootConfiguration(IOptions<AdminConfiguration> adminConfiguration)
  //      {
  //          AdminConfiguration = adminConfiguration.Value;
  //      }

		internal RootConfiguration()
		{
			AdminConfiguration = new AdminConfiguration();
			ConnectionStrings = new ConnectionStringsConfiguration();
		}
    }
}
