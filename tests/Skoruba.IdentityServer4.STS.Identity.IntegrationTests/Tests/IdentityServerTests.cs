using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests
{
    public class IdentityServerTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;

        public IdentityServerTests(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task CanShowDiscoveryEndpoint()
        {
            var disco = await _client.GetDiscoveryDocumentAsync("http://localhost");
  
            disco.Should().NotBeNull();
            disco.IsError.Should().Be(false);

            disco.KeySet.Keys.Count.Should().Be(1);
        }
    }
}
