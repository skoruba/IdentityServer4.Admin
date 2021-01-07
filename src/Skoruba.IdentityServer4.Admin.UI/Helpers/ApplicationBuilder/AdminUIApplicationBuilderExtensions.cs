using Microsoft.AspNetCore.Routing;
using Skoruba.IdentityServer4.Admin.UI.Helpers;

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
			// ...

			// Use Localization
			app.ConfigureLocalization();

			// ...

			return app;
		}

		///// <summary>
		///// Maps the Skoruba IdentityServer4 Admin UI to the routes of this application.
		///// </summary>
		///// <param name="endpoint"></param>
		//public static void MapIdentityServer4AdminUI(this IEndpointRouteBuilder endpoint)
		//{
		//	endpoint.MapDefaultControllerRoute();
		//	//endpoint.MapHealthChecks("/health", new HealthCheckOptions
		//	//{
		//	//	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		//	//});
		//}
	}
}
