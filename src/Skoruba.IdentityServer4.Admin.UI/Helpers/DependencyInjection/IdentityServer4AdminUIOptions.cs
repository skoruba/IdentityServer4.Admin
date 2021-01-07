using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.UI.Configuration;
using Skoruba.IdentityServer4.Admin.UI.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Configuration;
using Skoruba.IdentityServer4.Shared.Configuration.Common;
using System;
using System.Collections.Generic;

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
		/// The trusted domains from which content can be downloaded.
		/// </summary>
		public List<string> CspTrustedDomains { get; set; } = new List<string>();

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
	}
}
