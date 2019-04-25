using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.Helpers.Razor
{
	public class StaticFileProvidersConfigureOptions : IPostConfigureOptions<StaticFileOptions>
	{
		public StaticFileProvidersConfigureOptions(IHostingEnvironment environment)
		{
			Environment = environment;
		}
		public IHostingEnvironment Environment { get; }

		public void PostConfigure(string name, StaticFileOptions options)
		{
			name = name ?? throw new ArgumentNullException(nameof(name));
			options = options ?? throw new ArgumentNullException(nameof(options));

			// Basic initialization in case the options weren't initialized by any other component
			options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
			if (options.FileProvider == null && Environment.WebRootFileProvider == null)
			{
				throw new InvalidOperationException("Missing FileProvider.");
			}

			options.FileProvider = options.FileProvider ?? Environment.WebRootFileProvider;

			string basePath = "wwwroot";

			IFileProvider filesProvider = new EmbeddedFileProvider(GetType().Assembly, basePath);
			options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
		}
	}
}
