// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Original file: https://github.com/IdentityServer/IdentityServer4.Quickstart.UI
// Modified by Jan Škoruba
using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.STS.Identity.ViewModels.Account;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if the redirect URI is for a native client.
        /// </summary>
        /// <returns></returns>
        public static bool IsNativeClient(this AuthorizationRequest context)
        {
            return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
                   && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
        }

        public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
        {
            controller.HttpContext.Response.StatusCode = 200;
            controller.HttpContext.Response.Headers["Location"] = "";

            return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
        }

        public static NameValueCollection ToNameValueCollection(this NameValueCollection @this, string key)
        {
            var result = new NameValueCollection();

            if (@this != null && !string.IsNullOrEmpty(@this.Get(key)))
                result = @this.Get(key).ToNameValueCollection();

            return result;
        }
        public static NameValueCollection ToNameValueCollection(this string @this)
        {
            var result = new NameValueCollection();

            if (!string.IsNullOrEmpty(@this))
            {
                try
                {
                    var splitedraw = @this.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

                    foreach (var item in splitedraw)
                    {
                        string[] parts = item.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string val = parts[1].Trim();

                            result.Add(key, val);
                        }
                    }
                }
                catch (Exception) { }
            }

            return result;
        }
    }
}
