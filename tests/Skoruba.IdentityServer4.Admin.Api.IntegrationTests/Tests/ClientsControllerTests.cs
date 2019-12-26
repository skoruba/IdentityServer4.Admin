using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.Admin.Api.Configuration.Test;
using Skoruba.IdentityServer4.Admin.Api.IntegrationTests.Common;
using Skoruba.IdentityServer4.Admin.Api.IntegrationTests.Tests.Base;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.Api.IntegrationTests.Tests
{
    public class ClientsControllerTests : BaseClassFixture
    {
        public ClientsControllerTests(WebApplicationFactory<StartupTest> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetClientsAsAdmin()
        {
            SetupAdminClaimsViaHeaders();

            var response = await Client.GetAsync("api/clients");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetClientsWithoutPermissions()
        {
            Client.DefaultRequestHeaders.Clear();

            var response = await Client.GetAsync("api/clients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            response.Headers.Location.ToString().Should().Contain(AuthenticationConsts.AccountLoginPage);
        }
    }
}
