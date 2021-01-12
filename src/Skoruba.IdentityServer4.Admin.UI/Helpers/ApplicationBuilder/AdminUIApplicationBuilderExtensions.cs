using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.UI.Configuration;
using Skoruba.IdentityServer4.Admin.UI.Helpers;
using Skoruba.IdentityServer4.Admin.UI.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
	public static class AdminUIApplicationBuilderExtensions
	{
		/// <summary>
		/// Adds the Skoruba IdentityServer4 Admin UI to the pipeline of this application. This method must be called 
		/// between UseRouting() and UseEndpoints().
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseIdentityServer4AdminUI(this IApplicationBuilder app)
		{			
			app.UseRoutingDependentMiddleware(app.ApplicationServices.GetRequiredService<TestingConfiguration>());

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
