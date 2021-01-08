using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.Admin.Configuration.Test
{
	public class StartupTest : Startup
    {
        public StartupTest(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        public override void ConfigureUIOptions(IdentityServer4AdminUIOptions options)
        {
            // Applies configuration from appsettings.
            options.ApplyConfiguration(Configuration);
            options.ApplyConfiguration(HostingEnvironment);

            // Use staging DbContexts and auth services.
            options.IsStaging = true;
        }
    }
}
