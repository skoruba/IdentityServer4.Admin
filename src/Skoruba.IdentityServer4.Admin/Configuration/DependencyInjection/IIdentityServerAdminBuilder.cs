using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
	public interface IIdentityServerAdminBuilder
	{
		IServiceCollection Services { get; }

		IHostingEnvironment HostingEnvironment { get; }

		IConfigurationRoot ConfigurationRoot { get; }
	}
}
