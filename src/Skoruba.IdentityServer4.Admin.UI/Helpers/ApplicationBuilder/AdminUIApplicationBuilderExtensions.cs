using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.UI.Helpers;
using Skoruba.IdentityServer4.Admin.UI.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
	public static class AdminUIApplicationBuilderExtensions
	{
		/// <summary>
		/// Adds the Skoruba IdentityServer4 Admin UI to the pipeline of this application.
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseIdentityServer4AdminUI(this IApplicationBuilder app)
		{
			IdentityServer4AdminUIOptions options = app.ApplicationServices.GetRequiredService<IdentityServer4AdminUIOptions>();
			
			app.UseCookiePolicy();

			if (options.UseDeveloperExceptionPage)
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			if (options.UseHsts)
			{
				app.UseHsts();
			}

			app.UsePathBase(options.BasePath);

			// Add custom security headers
			app.UseSecurityHeaders(options.CspTrustedDomains);

			app.UseStaticFiles();

			app.UseAuthentication();
			if (options.IsStaging)
			{
				app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
			}

			// Use Localization
			app.ConfigureLocalization();

			return app;
		}

		/// <summary>
		/// Maps the Skoruba IdentityServer4 Admin UI to the routes of this application.
		/// </summary>
		/// <param name="endpoint"></param>
		public static void MapIdentityServer4AdminUI(this IEndpointRouteBuilder endpoint)
		{
			endpoint.MapDefaultControllerRoute();
			endpoint.MapHealthChecks("/health", new HealthCheckOptions
			{
				ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
			});
		}
	}
}
