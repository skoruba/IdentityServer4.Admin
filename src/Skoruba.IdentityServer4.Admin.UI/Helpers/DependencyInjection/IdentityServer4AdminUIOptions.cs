using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.UI.Configuration;
using Skoruba.IdentityServer4.Admin.UI.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Configuration;
using Skoruba.IdentityServer4.Shared.Configuration.Common;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
	public class IdentityServer4AdminUIOptions
	{
		/// <summary>
		/// Use test instead of production services and pipelines.
		/// </summary>
		public bool IsStaging { get; set; }

		/// <summary>
		/// The database connection strings and settings.
		/// </summary>
		public ConnectionStringsConfiguration ConnectionStrings { get; set; } = new ConnectionStringsConfiguration();

		/// <summary>
		/// The settings for the admin services.
		/// </summary>
		public AdminConfiguration Admin { get; set; } = new AdminConfiguration();

		/// <summary>
		/// The settings for the database provider.
		/// </summary>
		public DatabaseProviderConfiguration DatabaseProvider { get; set; } = new DatabaseProviderConfiguration();

		/// <summary>
		/// The settings for audit logging.
		/// </summary>
		public AuditLoggingConfiguration AuditLogging { get; set; } = new AuditLoggingConfiguration();

		/// <summary>
		/// The settings for globalization.
		/// </summary>
		public CultureConfiguration Culture { get; set; } = new CultureConfiguration();

		/// <summary>
		/// An action to configure ASP.NET Core Identity.
		/// </summary>
		public Action<IdentityOptions> IdentityAction { get; set; } = options => { };

		/// <summary>
		/// The settings for data protection.
		/// </summary>
		public DataProtectionConfiguration DataProtection { get; set; } = new DataProtectionConfiguration();

		/// <summary>
		/// The settings for Azure key vault.
		/// </summary>
		public AzureKeyVaultConfiguration AzureKeyVault { get; set; } = new AzureKeyVaultConfiguration();

		/// <summary>
		/// Identity data to seed the databases.
		/// </summary>
		public IdentityDataConfiguration IdentityData { get; set; } = new IdentityDataConfiguration();

		/// <summary>
		/// Identity server data to seed the dtabases.
		/// </summary>
		public IdentityServerDataConfiguration IdentityServerData { get; set; } = new IdentityServerDataConfiguration();

		#region App Building
		/// <summary>
		/// The trusted domains from which content can be downloaded.
		/// </summary>
		public List<string> CspTrustedDomains { get; set; } = new List<string>();

		public string BasePath { get; set; } = "";

		/// <summary>
		/// Use the developer exception page instead of the Identity error handler.
		/// </summary>
		public bool UseDeveloperExceptionPage { get; set; } = false;

		public bool UseHsts { get; set; } = true; 
		#endregion

		/// <summary>
		/// Applies configuration parsed from an appsettings file into these options.
		/// </summary>
		/// <param name="configuration">The configuration to bind into this instance.</param>
		public void ApplyConfiguration(IConfiguration configuration)
		{
			configuration.GetSection(ConfigurationConsts.ConnectionStringsKey).Bind(ConnectionStrings);
			configuration.GetSection(nameof(AdminConfiguration)).Bind(Admin);
			configuration.GetSection(nameof(DatabaseProviderConfiguration)).Bind(DatabaseProvider);
			configuration.GetSection(nameof(AuditLoggingConfiguration)).Bind(AuditLogging);
			configuration.GetSection(nameof(CultureConfiguration)).Bind(Culture);
			configuration.GetSection(nameof(DataProtectionConfiguration)).Bind(DataProtection);
			configuration.GetSection(nameof(AzureKeyVaultConfiguration)).Bind(AzureKeyVault);
			IdentityAction = options => configuration.GetSection(nameof(IdentityOptions)).Bind(options);
			configuration.GetSection(ConfigurationConsts.CspTrustedDomainsKey).Bind(CspTrustedDomains);
		}

		public void ApplyConfiguration(IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				UseDeveloperExceptionPage = true;
				UseHsts = false;
			}
			else
			{
				UseDeveloperExceptionPage = false;
				UseHsts = true;
			}
		}
	}
}
