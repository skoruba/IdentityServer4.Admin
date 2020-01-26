using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.TestsMultiTenant.Base
{
    public class BaseClassFixture : IClassFixture<WebApplicationFactory<StartupTestMultiTenant>>
    {
        protected readonly HttpClient Client;
        public BaseClassFixture(WebApplicationFactory<StartupTestMultiTenant> factory)
        {
            Client = factory.SetupClient();
        }
    }
}