using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests
{
    public class HomeControllerTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;

        public HomeControllerTests(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task EveryoneHasAccessToHomepage()
        {
            _client.DefaultRequestHeaders.Clear();

            // Act
            var response = await _client.GetAsync("/home/index");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}