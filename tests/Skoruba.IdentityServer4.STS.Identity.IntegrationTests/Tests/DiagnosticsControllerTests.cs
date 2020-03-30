using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests.Base;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests
{
    public class DiagnosticsControllerTests : BaseClassFixture
    {
        public DiagnosticsControllerTests(WebApplicationFactory<StartupTest> factory) : base(factory)
        {
        }

        [Fact]
        public async Task UnAuthorizeUserCannotAccessDiagnosticsView()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Act
            var response = await Client.GetAsync("/Diagnostics/Index");

            // Assert      
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            response.Headers.Location.ToString().Should().Contain("Account/Login");
        }
    }
}
