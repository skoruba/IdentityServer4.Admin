using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.Configuration;
using System;
using System.Collections.Generic;
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

		public void SetConnection(string adminConnectionString)
			=> SetConnection(adminConnectionString, adminConnectionString, adminConnectionString, adminConnectionString);

		public void SetConnection(string configurationConnectionString, string persistedGrantConnectionString, string identityConnectionString, string adminLogConnectionString)
		{
			RootConfiguration.ConnectionStrings.ConfigurationDbConnection = configurationConnectionString;
			RootConfiguration.ConnectionStrings.PersistedGrantDbConnection = persistedGrantConnectionString;
			RootConfiguration.ConnectionStrings.IdentityDbConnection = identityConnectionString;
			RootConfiguration.ConnectionStrings.AdminLogDbConnection = adminLogConnectionString;
		}

		public void ApplyConfiguration(IConfigurationRoot configurationRoot)
		{
			configurationRoot.Bind(RootConfiguration);
		}
	}
}
