using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Mocks;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests.Base;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests
{
    public class GrantsControllerTests : BaseClassFixture
    {
        public GrantsControllerTests(WebApplicationFactory<StartupTest> factory) : base(factory)
        {
        }

        [Fact]
        public async Task AuthorizeUserCanAccessGrantsView()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Get cookie with user identity for next request
            Client.PutCookiesOnRequest(registerResponse);

            // Act
            var response = await Client.GetAsync("/Grants/Index");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UnAuthorizeUserCannotAccessGrantsView()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Act
            var response = await Client.GetAsync("/Grants/Index");

            // Assert      
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            response.Headers.Location.ToString().Should().Contain("Account/Login");
        }
    }
}
