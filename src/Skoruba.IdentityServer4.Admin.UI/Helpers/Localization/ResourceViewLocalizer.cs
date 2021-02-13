using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Skoruba.IdentityServer4.Admin.UI.Helpers.Localization
{
	public class ResourceViewLocalizer : IViewLocalizer, IViewContextAware
	{
		// This class helps working around https://github.com/aspnet/Localization/issues/328:
		// Somehow the resources in this library project fail to be resolved by the default view localizer (which 
		// relies on the hosting environment's application name as a root path for the resources).
		// We override this behaviour by forcing .NET to look into our assembly's Resources and fallback to the default
		// view localizer for localization requests related to views outside of this library.

		private class MockEnvironment : IWebHostEnvironment
		{
			// This fake environment implementation helps us leveraging .NET's default ViewLocalizer implementation 
			// without having to rewrite it completely. The only used property is ApplicationName. The other properties 
			// should be unused.

			public string ApplicationName { get; set; }

			#region Unused
			public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
			public string WebRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
			public string EnvironmentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
			public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
			public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

			#endregion
		}

		private readonly ViewLocalizer _internalViewLocalizer;
		private readonly ViewLocalizer _defaultViewLocalizer;
		private readonly string _assemblyName;

		public ResourceViewLocalizer(IHtmlLocalizerFactory localizerFactory, IWebHostEnvironment hostingEnvironment)
		{
			if (localizerFactory == null)
			{
				throw new ArgumentNullException(nameof(localizerFactory));
			}

			if (hostingEnvironment == null)
			{
				throw new ArgumentNullException(nameof(hostingEnvironment));
			}

			_assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			_defaultViewLocalizer = new ViewLocalizer(localizerFactory, hostingEnvironment);
			_internalViewLocalizer = new ViewLocalizer(localizerFactory, new MockEnvironment() { ApplicationName = _assemblyName });
		}

		public LocalizedHtmlString this[string name]
		{
			get
			{
				// Resolves the resources into the default localizer first (to allow resource overriding by library 
				// consumers).
				LocalizedHtmlString str = _defaultViewLocalizer[name];
				return str.IsResourceNotFound ? _internalViewLocalizer[name] : str;
			}
		}

		public LocalizedHtmlString this[string name, params object[] arguments]
		{
			get
			{
				// Resolves the resources into the default localizer first (to allow resource overriding by library 
				// consumers).
				LocalizedHtmlString str = _defaultViewLocalizer[name, arguments];
				return str.IsResourceNotFound ? _internalViewLocalizer[name, arguments] : str;
			}
		}

		public void Contextualize(ViewContext viewContext)
		{
			_internalViewLocalizer.Contextualize(viewContext);
			_defaultViewLocalizer.Contextualize(viewContext);
		}

		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		{
			return _defaultViewLocalizer.GetAllStrings(includeParentCultures)
				.Union(_internalViewLocalizer.GetAllStrings(includeParentCultures));
		}

		public LocalizedString GetString(string name)
		{
			// Resolves the resources into the default localizer first (to allow resource overriding by library 
			// consumers).
			LocalizedString str = _defaultViewLocalizer.GetString(name);
			return str.ResourceNotFound ? _internalViewLocalizer.GetString(name) : str;
		}

		public LocalizedString GetString(string name, params object[] arguments)
		{
			// Resolves the resources into the default localizer first (to allow resource overriding by library 
			// consumers).
			LocalizedString str = _defaultViewLocalizer.GetString(name, arguments);
			return str.ResourceNotFound ? _internalViewLocalizer.GetString(name, arguments) : str;
		}

		[Obsolete("This method is obsolete. Use `CurrentCulture` and `CurrentUICulture` instead.")]
		public IHtmlLocalizer WithCulture(CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
