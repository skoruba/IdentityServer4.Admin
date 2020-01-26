using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Configuration.Test;
using Skoruba.IdentityServer4.Admin.IntegrationTests.Tests.Base;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.IntegrationTests.Tests
{
    public class LogControllerTests : BaseClassFixture
    {
        public LogControllerTests(WebApplicationFactory<StartupTest> factory) : base(factory)
        {
        }

        [Fact]
        public async Task ReturnRedirectInErrorsLogWithoutAdminRole()
        {
            //Remove
            Client.DefaultRequestHeaders.Clear();

            // Act
            var response = await Client.GetAsync("/log/errorslog");

            // Assert           
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            response.Headers.Location.ToString().Should().Contain(AuthenticationConsts.AccountLoginPage);
        }

        [Fact]
        public async Task ReturnRedirectInAuditLogWithoutAdminRole()
        {
            //Remove
            Client.DefaultRequestHeaders.Clear();

            // Act
            var response = await Client.GetAsync("/log/auditlog");

            // Assert           
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            response.Headers.Location.ToString().Should().Contain(AuthenticationConsts.AccountLoginPage);
        }

        [Fact]
        public async Task ReturnSuccessInErrorsLogWithAdminRole()
        {
            SetupAdminClaimsViaHeaders();

            // Act
            var response = await Client.GetAsync("/log/errorslog");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnSuccessInAuditLogWithAdminRole()
        {
            SetupAdminClaimsViaHeaders();

            // Act
            var response = await Client.GetAsync("/log/auditlog");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
