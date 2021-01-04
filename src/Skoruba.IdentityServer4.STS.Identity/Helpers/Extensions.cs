// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Original file: https://github.com/IdentityServer/IdentityServer4.Quickstart.UI
// Modified by Jan Škoruba

using IdentityServer4.Stores;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Determines whether the client is configured to use PKCE.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="client_id">The client identifier.</param>
        /// <returns></returns>
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string client_id)
        {
            if (!string.IsNullOrWhiteSpace(client_id))
            {
                var client = await store.FindEnabledClientByIdAsync(client_id);
                return client?.RequirePkce == true;
            }

            return false;
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
