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

			app.UseSecurityHeaders();

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = StartupHelpers.GetEmbeddedFileProvider("wwwroot")
			});

			app.ConfigureAuthenticationServices(options);
			app.ConfigureLocalization();

			app.UseMvc(routes =>
			{
				routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});

			return app;
		}
	}
}
