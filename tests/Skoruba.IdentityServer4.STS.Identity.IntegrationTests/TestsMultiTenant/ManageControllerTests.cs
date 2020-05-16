﻿using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Mocks;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.TestsMultiTenant.Base;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.TestsMultiTenant
{
    public class ManageControllerTests : BaseClassFixture
    {
        public ManageControllerTests(WebApplicationFactory<StartupTestMultiTenant> factory) : base(factory)
        {
        }

        [Fact]
        public async Task AuthorizeUserCanAccessManageViews()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Prepare request to login
            const string accountLoginAction = "/Account/Login";
            var loginResponse = await Client.GetAsync(accountLoginAction);
            var antiForgeryToken = await loginResponse.ExtractAntiForgeryToken();

            // User Guid like fake password
            var loginDataForm = UserMocks.GenerateLoginData(registerFormData["UserName"], registerFormData["Password"], registerFormData["TenantCode"], antiForgeryToken);

            // Login
            var requestMessage = RequestHelper.CreatePostRequestWithCookies(accountLoginAction, loginDataForm, loginResponse);
            var responseMessage = await Client.SendAsync(requestMessage);


            // Get cookie with user identity for next request
            Client.PutCookiesOnRequest(responseMessage);

            foreach (var route in RoutesConstants.GetManageRoutes())
            {
                // Act
                var response = await Client.GetAsync($"/Manage/{route}");

                // Assert
                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task UnAuthorizeUserCannotAccessManageViews()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            foreach (var route in RoutesConstants.GetManageRoutes())
            {
                // Act
                var response = await Client.GetAsync($"/Manage/{route}");

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
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Get cookie with user identity for next request
            Client.PutCookiesOnRequest(registerResponse);

            // Prepare request to update profile
            const string manageAction = "/Manage/Index";
            var manageResponse = await Client.GetAsync(manageAction);
            var antiForgeryToken = await manageResponse.ExtractAntiForgeryToken();

            var manageProfileData = UserMocks.GenerateManageProfileData(registerFormData["Email"], antiForgeryToken);

            // Update profile
            var requestWithAntiForgeryCookie = RequestHelper.CreatePostRequestWithCookies(manageAction, manageProfileData, manageResponse);
            var requestWithIdentityCookie = CookiesHelper.CopyCookiesFromResponse(requestWithAntiForgeryCookie, registerResponse);
            var responseMessage = await Client.SendAsync(requestWithIdentityCookie);

            // Assert      
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            responseMessage.Headers.Location.ToString().Should().Be("/Manage");
        }
    }
}
