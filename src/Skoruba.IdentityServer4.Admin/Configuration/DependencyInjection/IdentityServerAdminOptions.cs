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

		public void ApplyConfiguration(IConfigurationRoot configurationRoot)
		{
			configurationRoot.Bind(RootConfiguration);
		}
	}
}
