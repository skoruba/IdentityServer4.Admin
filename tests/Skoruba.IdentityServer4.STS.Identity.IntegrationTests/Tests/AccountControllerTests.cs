using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlAgilityPack;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests
{
    public class AccountControllerTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;

        public AccountControllerTests(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task UserIsAbleToRegister()
        {
            // Clear headers
            _client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(_client, registerFormData);

            // Assert      
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            registerResponse.Headers.Location.ToString().Should().Be("/");
        }

        [Fact]
        public async Task UserIsNotAbleToRegisterWithSameUserName()
        {
            // Clear headers
            _client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();

            var registerResponseFirst = await UserMocks.RegisterNewUserAsync(_client, registerFormData);
            
            // Assert      
            registerResponseFirst.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            registerResponseFirst.Headers.Location.ToString().Should().Be("/");

            var registerResponseSecond = await UserMocks.RegisterNewUserAsync(_client, registerFormData);

            // Assert response
            registerResponseSecond.StatusCode.Should().Be(HttpStatusCode.OK);

            // Get html content
            var contentWithErrorMessage = await registerResponseSecond.Content.ReadAsStringAsync();

            // From String
            var doc = new HtmlDocument();
            doc.LoadHtml(contentWithErrorMessage);

            // Get error messages from validation summary
            var errorNodes = doc.DocumentNode
                .SelectNodes("//div[contains(@class, 'validation-summary-errors')]/ul/li");

            errorNodes.Should().HaveCount(2);

            // Build expected error messages
            var expectedErrorMessages = new List<string>
            {
                $"User name &#x27;{registerFormData["UserName"]}&#x27; is already taken.",
                $"Email &#x27;{registerFormData["Email"]}&#x27; is already taken."
            };

            // Assert
            var containErrors = errorNodes.Select(x => x.InnerText).ToList().SequenceEqual(expectedErrorMessages);

            containErrors.Should().BeTrue();
        }
    }
}
