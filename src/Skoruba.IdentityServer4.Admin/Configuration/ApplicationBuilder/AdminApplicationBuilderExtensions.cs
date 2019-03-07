using Microsoft.Extensions.FileProviders;
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
			Assembly executingAssembly = Assembly.GetExecutingAssembly();

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new EmbeddedFileProvider(executingAssembly, executingAssembly.GetName().Name + ".wwwroot")
			});

			return app;
		}
	}
}
