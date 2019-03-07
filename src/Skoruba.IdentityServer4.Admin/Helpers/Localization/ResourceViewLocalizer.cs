// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// https://github.com/aspnet/Mvc/blob/release/2.2/src/Microsoft.AspNetCore.Mvc.Localization/ViewLocalizer.cs
// Modified by Brice Clocher

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;

namespace Skoruba.IdentityServer4.Admin.Helpers.Localization
{
	internal class ResourceViewLocalizer : IViewLocalizer, IViewContextAware
	{
		private readonly IHtmlLocalizerFactory _localizerFactory;
		private readonly string _assemblyName;
		private IHtmlLocalizer _localizer;

		/// <summary>
		/// Creates a new <see cref="ViewLocalizer"/>.
		/// </summary>
		/// <param name="localizerFactory">The <see cref="IHtmlLocalizerFactory"/>.</param>
		public ResourceViewLocalizer(IHtmlLocalizerFactory localizerFactory)
		{
			if (localizerFactory == null)
			{
				throw new ArgumentNullException(nameof(localizerFactory));
			}

			_assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			_localizerFactory = localizerFactory;
		}

		/// <inheritdoc />
		public virtual LocalizedHtmlString this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				return _localizer[key];
			}
		}

		/// <inheritdoc />
		public virtual LocalizedHtmlString this[string key, params object[] arguments]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				return _localizer[key, arguments];
			}
		}

		/// <inheritdoc />
		public LocalizedString GetString(string name) => _localizer.GetString(name);

		/// <inheritdoc />
		public LocalizedString GetString(string name, params object[] values) => _localizer.GetString(name, values);

		/// <inheritdoc />
		public IHtmlLocalizer WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);

		/// <inheritdoc />
		public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
			_localizer.GetAllStrings(includeParentCultures);

		/// <summary>
		/// Apply the specified <see cref="ViewContext"/>.
		/// </summary>
		/// <param name="viewContext">The <see cref="ViewContext"/>.</param>
		public void Contextualize(ViewContext viewContext)
		{
			if (viewContext == null)
			{
				throw new ArgumentNullException(nameof(viewContext));
			}

			// Given a view path "/Views/Home/Index.cshtml" we want a baseName like "MyApplication.Views.Home.Index"
			var path = viewContext.ExecutingFilePath;

			if (string.IsNullOrEmpty(path))
			{
				path = viewContext.View.Path;
			}

			Debug.Assert(!string.IsNullOrEmpty(path), "Couldn't determine a path for the view");

			_localizer = _localizerFactory.Create(BuildBaseName(path), _assemblyName);
		}

		private string BuildBaseName(string path)
		{
			var extension = Path.GetExtension(path);
			var startIndex = path[0] == '/' || path[0] == '\\' ? 1 : 0;
			var length = path.Length - startIndex - extension.Length;
			var capacity = length + _assemblyName.Length + 1;
			var builder = new StringBuilder(path, startIndex, length, capacity);

			builder.Replace('/', '.').Replace('\\', '.');

			// Prepend the application name
			builder.Insert(0, '.');
			builder.Insert(0, _assemblyName);

			return builder.ToString();
		}
	}
}