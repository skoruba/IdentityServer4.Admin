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
    public class IdentityResourcesControllerTests : BaseClassFixture
    {
        public IdentityResourcesControllerTests(WebApplicationFactory<StartupTest> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetIdentityResourcesAsAdmin()
        {
            SetupAdminClaimsViaHeaders();

            var response = await Client.GetAsync("api/identityresources");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetIdentityResourcesWithoutPermissions()
        {
            Client.DefaultRequestHeaders.Clear();

            var response = await Client.GetAsync("api/identityresources");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            response.Headers.Location.ToString().Should().Contain(AuthenticationConsts.AccountLoginPage);
        }
    }
}