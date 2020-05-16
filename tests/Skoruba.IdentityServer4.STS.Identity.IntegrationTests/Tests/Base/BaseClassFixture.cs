using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
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
}