using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.TestsMultiTenant.Base;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.TestsMultiTenant
{
    public class HomeControllerTests : BaseClassFixture
    {
        public HomeControllerTests(WebApplicationFactory<StartupTestMultiTenant> factory) : base(factory)
        {
        }

        [Fact]
        public async Task EveryoneHasAccessToHomepage()
        {
            Client.DefaultRequestHeaders.Clear();

            // Act
            var response = await Client.GetAsync("/home/index");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}