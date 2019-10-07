using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests.Base
{
    public class BaseClassFixture : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected readonly HttpClient _client;

        public BaseClassFixture(WebApplicationFactory<Startup> factory)
        {
            _client = factory.SetupClient();
        }
    }
}