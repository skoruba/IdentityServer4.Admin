using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
	public static class AdminApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseIdentityServerAdminUI(this IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, Microsoft.Extensions.Configuration.IConfigurationRoot configuration)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();

			app.AddLogging(loggerFactory, configuration);

			if (env.IsDevelopment())
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
				FileProvider = new EmbeddedFileProvider(executingAssembly, executingAssembly.GetName().Name + ".wwwroot")
			});

			app.ConfigureAuthenticationServices(env);
			app.ConfigureLocalization();

			app.UseMvc(routes =>
			{
				routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});

			return app;
		}
	}
}
