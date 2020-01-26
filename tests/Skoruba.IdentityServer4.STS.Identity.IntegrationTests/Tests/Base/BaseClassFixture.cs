using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests.Base
{
    public class BaseClassFixture : IClassFixture<WebApplicationFactory<StartupTestSingleTenant>>
    {
        protected readonly HttpClient Client;

        public BaseClassFixture(WebApplicationFactory<StartupTestSingleTenant> factory)
        {
            Client = factory.SetupClient();
        }
    }

    public class BaseClassMultiTenantFixture : IClassFixture<WebApplicationFactory<StartupTestMultiTenant>>
    {
        protected readonly HttpClient Client;

        public BaseClassMultiTenantFixture(WebApplicationFactory<StartupTestMultiTenant> factory)
        {
            Client = factory.SetupClient();
        }
    }
}