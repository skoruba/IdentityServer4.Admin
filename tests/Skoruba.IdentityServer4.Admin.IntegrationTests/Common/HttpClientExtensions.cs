﻿using System.Net.Http;
using Skoruba.IdentityServer4.Admin.Middlewares;
using Skoruba.IdentityServer4.Shared.Configuration.Constants;

namespace Skoruba.IdentityServer4.Admin.IntegrationTests.Common
{
    public static class HttpClientExtensions
    {
        public static void SetAdminClaimsViaHeaders(this HttpClient client)
        {
            client.DefaultRequestHeaders.Add($"{AuthenticatedTestRequestMiddleware.TestUserPrefixHeader}-{AuthenticatedTestRequestMiddleware.TestUserId}", "1");
            client.DefaultRequestHeaders.Add($"{AuthenticatedTestRequestMiddleware.TestUserPrefixHeader}-{AuthenticatedTestRequestMiddleware.TestUserName}", "test");
            client.DefaultRequestHeaders.Add($"{AuthenticatedTestRequestMiddleware.TestUserPrefixHeader}-{AuthenticatedTestRequestMiddleware.TestUserRoles}", AuthorizationConsts.AdministrationRole);
        }

    }
}
