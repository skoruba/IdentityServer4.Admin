using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Shared.Dtos;
using Skoruba.IdentityServer4.Shared.Dtos.Identity;
using Skoruba.IdentityServer4.Shared.Helpers;

namespace Skoruba.IdentityServer4.Admin
{
	public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            HostingEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adds the IdentityServer4 Admin UI with custom options.
            services.AddIdentityServer4AdminUI(ConfigureUIOptions);

            // Add email senders which is currently setup for SendGrid and SMTP
            //services.AddEmailSenders(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
			app.UseRouting();

            app.UseIdentityServer4AdminUI();

            app.UseEndpoints(endpoint =>
            {
				endpoint.MapIdentityServer4AdminUI();
                endpoint.MapIdentityServer4AdminUIHealthChecks();
            });
        }

        public virtual void ConfigureUIOptions(IdentityServer4AdminUIOptions options)
		{
            // Applies configuration from appsettings.
            options.BindConfiguration(Configuration);
			if (HostingEnvironment.IsDevelopment())
			{
                options.Security.UseDeveloperExceptionPage = true;
			}
			else
			{
                options.Security.UseHsts = true;
			}
            
            // Use production DbContexts and auth services.
            options.Testing.IsStaging = false;
		}
    }
}