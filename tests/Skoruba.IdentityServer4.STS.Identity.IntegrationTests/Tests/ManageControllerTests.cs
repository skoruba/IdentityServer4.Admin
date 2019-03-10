using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests
{
    public class ManageControllerTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;

        public ManageControllerTests(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task AuthorizeUserCanAccessManageViews()
        {
            // Clear headers
            _client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(_client,registerFormData);

            // Get cookie with user identity for next request
            _client.PutCookiesOnRequest(registerResponse);
            
            foreach (var route in RoutesConstants.GetManageRoutes())
            {
                // Act
                var response = await _client.GetAsync($"/Manage/{route}");

                // Assert
                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task UnAuthorizeUserCannotAccessManageViews()
        {
            // Clear headers
            _client.DefaultRequestHeaders.Clear();

            foreach (var route in RoutesConstants.GetManageRoutes())
            {
                // Act
                var response = await _client.GetAsync($"/Manage/{route}");

                // Assert      
                response.StatusCode.Should().Be(HttpStatusCode.Redirect);

                //The redirect to login
                response.Headers.Location.ToString().Should().Contain("Account/Login");
            }
        }
        
        [Fact]
        public async Task UserIsAbleToUpdateProfile()
        {
            // Clear headers
            _client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(_client, registerFormData);

            // Get cookie with user identity for next request
            _client.PutCookiesOnRequest(registerResponse);

            // Prepare request to update profile
            const string manageAction = "/Manage/Index";
            var manageResponse = await _client.GetAsync(manageAction);
            var antiForgeryToken = await manageResponse.ExtractAntiForgeryToken();

            var manageProfileData = UserMocks.GenerateManageProfileData(registerFormData["Email"], antiForgeryToken);

            // Update profile
            var requestWithAntiForgeryCookie = RequestHelper.CreatePostRequestWithCookies(manageAction, manageProfileData, manageResponse);
            var requestWithIdentityCookie = CookiesHelper.CopyCookiesFromResponse(requestWithAntiForgeryCookie, registerResponse);
            var responseMessage = await _client.SendAsync(requestWithIdentityCookie);

            // Assert      
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            responseMessage.Headers.Location.ToString().Should().Be("/Manage");
        }
    }
}
