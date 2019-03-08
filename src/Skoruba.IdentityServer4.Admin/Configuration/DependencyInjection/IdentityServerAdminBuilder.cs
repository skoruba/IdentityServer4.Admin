using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	internal class IdentityServerAdminBuilder : IIdentityServerAdminBuilder
	{
		public IServiceCollection Services { get; }

		public IHostingEnvironment HostingEnvironment { get; }

		public IConfigurationRoot ConfigurationRoot { get; }

		public IdentityServerAdminBuilder(IServiceCollection services, IHostingEnvironment env, IConfigurationRoot configurationRoot)
		{
			Services = services ?? throw new ArgumentNullException(nameof(services));
			HostingEnvironment = env ?? throw new ArgumentNullException(nameof(env));
			ConfigurationRoot = configurationRoot ?? throw new ArgumentNullException(nameof(configurationRoot));
		}
	}
}
