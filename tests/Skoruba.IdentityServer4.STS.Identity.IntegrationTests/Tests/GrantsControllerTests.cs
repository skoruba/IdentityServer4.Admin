using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests
{
    public class GrantsControllerTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;

        public GrantsControllerTests(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task AuthorizeUserCanAccessGrantsView()
        {
            // Clear headers
            _client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(_client, registerFormData);

            // Get cookie with user identity for next request
            _client.PutCookiesOnRequest(registerResponse);

            // Act
            var response = await _client.GetAsync("/Grants/Index");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UnAuthorizeUserCannotAccessGrantsView()
        {
            // Clear headers
            _client.DefaultRequestHeaders.Clear();

            // Act
            var response = await _client.GetAsync("/Grants/Index");

            // Assert      
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            response.Headers.Location.ToString().Should().Contain("Account/Login");
        }
    }
}
