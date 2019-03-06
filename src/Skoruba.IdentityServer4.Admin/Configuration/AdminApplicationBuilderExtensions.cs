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
			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "Skoruba.IdentityServer4.Admin.wwwroot")
			});

			return app;
		}
	}
}
