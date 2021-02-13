using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.Configuration.Test;
using Skoruba.IdentityServer4.Admin.IntegrationTests.Common;
using Skoruba.IdentityServer4.Admin.UI.Configuration;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.IntegrationTests.Tests.Base
{
	public class BaseClassFixture : IClassFixture<WebApplicationFactory<StartupTest>>
    {
        protected readonly WebApplicationFactory<StartupTest> Factory;
        protected readonly HttpClient Client;

        public BaseClassFixture(WebApplicationFactory<StartupTest> factory)
        {
            Factory = factory;
            Client = factory.SetupClient();
            Factory.CreateClient();
        }

        protected virtual void SetupAdminClaimsViaHeaders()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<AdminConfiguration>();
                Client.SetAdminClaimsViaHeaders(configuration);
            }
        }
    }
}