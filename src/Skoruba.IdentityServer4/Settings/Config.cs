// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Skoruba.IdentityServer4.Constants;

namespace Skoruba.IdentityServer4.Settings
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles", new[] { "role" })
            };
        }

        // scopes define the resources in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>();
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = IdentityServerConsts.OidcClientId,
                    ClientName = IdentityServerConsts.OidcClientName,
                    ClientUri = IdentityServerConsts.IdentityAdminBaseUrl,

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{IdentityServerConsts.IdentityAdminBaseUrl}/signin-oidc"},
                    FrontChannelLogoutUri = $"{IdentityServerConsts.IdentityAdminBaseUrl}/signout-oidc",
                    PostLogoutRedirectUris = { $"{IdentityServerConsts.IdentityAdminBaseUrl}/signout-callback-oidc"},
                    AllowedCorsOrigins = { IdentityServerConsts.IdentityAdminBaseUrl },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles"
                    }
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "bob",
                    Password = "Pa$$word123",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Bob"),
                        new Claim("role", IdentityServerConsts.AdministrationRole)
                    }
                }
            };
        }
    }
}