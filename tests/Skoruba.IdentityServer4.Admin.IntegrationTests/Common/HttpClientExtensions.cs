using System.Net.Http;
using Skoruba.IdentityServer4.Admin.Common.Settings;
using Skoruba.IdentityServer4.Admin.Constants;
using Skoruba.IdentityServer4.Admin.Middlewares;

namespace Skoruba.IdentityServer4.Admin.IntegrationTests.Common
{
    public static class HttpClientExtensions
    {
        static AdminAppSettings _defaultSettings = new AdminAppSettings();

        public static void SetAdminClaimsViaHeaders(this HttpClient client)
        {
            client.DefaultRequestHeaders.Add($"{AuthenticatedTestRequestMiddleware.TestUserPrefixHeader}-{AuthenticatedTestRequestMiddleware.TestUserId}", "1");
            client.DefaultRequestHeaders.Add($"{AuthenticatedTestRequestMiddleware.TestUserPrefixHeader}-{AuthenticatedTestRequestMiddleware.TestUserName}", "test");
            client.DefaultRequestHeaders.Add($"{AuthenticatedTestRequestMiddleware.TestUserPrefixHeader}-{AuthenticatedTestRequestMiddleware.TestUserRoles}", _defaultSettings.AdministrationRole);
        }

    }
}
