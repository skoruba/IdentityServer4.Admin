using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.IntegrationTests.Common;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.IntegrationTests.Tests
{
    public class GrantControllerTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;

        public GrantControllerTests(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task ReturnSuccessWithAdminRole()
        {
            //Get claims for admin
            _client.SetAdminClaimsViaHeaders();

            foreach (var route in RoutesConstants.GetGrantRoutes())
            {
                // Act
                var response = await _client.GetAsync($"/Grant/{route}");

                // Assert
                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task ReturnRedirectWithoutAdminRole()
        {
            //Remove
            _client.DefaultRequestHeaders.Clear();

            foreach (var route in RoutesConstants.GetGrantRoutes())
            {
                // Act
                var response = await _client.GetAsync($"/Grant/{route}");
                
                // Assert           
                response.StatusCode.Should().Be(HttpStatusCode.Redirect);

                //The redirect to login
                response.Headers.Location.ToString().Should().Contain(AuthenticationConsts.AccountLoginPage);
            }
        }
    }
}
