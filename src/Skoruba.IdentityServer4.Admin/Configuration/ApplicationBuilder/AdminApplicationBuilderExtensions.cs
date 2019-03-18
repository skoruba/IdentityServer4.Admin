using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.Configuration;
using Skoruba.IdentityServer4.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
	public static class AdminApplicationBuilderExtensions
	{
		/// <summary>
		/// Adds the Skoruba Identity Server Admin UI to the pipeline of this application.
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseIdentityServerAdminUI(this IApplicationBuilder app)
		{
			IdentityServerAdminOptions options = app.ApplicationServices.GetService<IdentityServerAdminOptions>();

			app.UseLogging(options);

			if (options.UseDeveloperExceptionPage)
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			// Add custom security headers
			app.UseSecurityHeaders();

			// Add serving of static files from this assembly's wwwroot folder.
			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = StartupHelpers.GetEmbeddedFileProvider("wwwroot")
			});

			// Use authentication and for integration tests use custom middleware which is used only in Staging environment
			app.ConfigureAuthenticationServices(options);

			// Use Localization
			app.ConfigureLocalization();

			app.UseMvc(routes =>
			{
				routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});

			return app;
		}
	}
}
