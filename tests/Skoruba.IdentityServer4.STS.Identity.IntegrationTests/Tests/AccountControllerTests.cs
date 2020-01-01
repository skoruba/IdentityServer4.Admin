using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Mocks;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests.Base;
using Xunit;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests
{
    public class AccountControllerTests : BaseClassFixture
    {
        public AccountControllerTests(WebApplicationFactory<StartupTest> factory) : base(factory)
        {
        }

        [Fact]
        public async Task UserIsAbleToRegister()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Assert      
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            registerResponse.Headers.Location.ToString().Should().Be("/");
        }

        [Fact]
        public async Task UserIsNotAbleToRegisterWithSameUserName()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();

            var registerResponseFirst = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Assert      
            registerResponseFirst.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            registerResponseFirst.Headers.Location.ToString().Should().Be("/");

            var registerResponseSecond = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

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

        [Fact]
        public async Task UserIsAbleToLogin()
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

            var loginDataForm = UserMocks.GenerateLoginData(registerFormData["UserName"], registerFormData["Password"],
                antiForgeryToken);

            // Login
            var requestMessage = RequestHelper.CreatePostRequestWithCookies(accountLoginAction, loginDataForm, loginResponse);
            var responseMessage = await Client.SendAsync(requestMessage);

            // Assert status code    
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Redirect);

            // Assert redirect location
            responseMessage.Headers.Location.ToString().Should().Be("/");

            // Check if response contain cookie with Identity
            const string identityCookieName = ".AspNetCore.Identity.Application";
            var existsCookie = CookiesHelper.ExistsCookie(responseMessage, identityCookieName);

            // Assert Identity cookie
            existsCookie.Should().BeTrue();
        }

        [Fact]
        public async Task UserIsNotAbleToLoginWithIncorrectPassword()
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
            var loginDataForm = UserMocks.GenerateLoginData(registerFormData["UserName"], Guid.NewGuid().ToString(), antiForgeryToken);

            // Login
            var requestMessage = RequestHelper.CreatePostRequestWithCookies(accountLoginAction, loginDataForm, loginResponse);
            var responseMessage = await Client.SendAsync(requestMessage);

            // Get html content
            var contentWithErrorMessage = await responseMessage.Content.ReadAsStringAsync();

            // Assert status code    
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            // From String
            var doc = new HtmlDocument();
            doc.LoadHtml(contentWithErrorMessage);

            // Get error messages from validation summary
            var errorNodes = doc.DocumentNode
                .SelectNodes("//div[contains(@class, 'validation-summary-errors')]/ul/li");

            errorNodes.Should().HaveCount(1);

            // Build expected error messages
            var expectedErrorMessages = new List<string>
            {
                "Invalid username or password"
            };

            // Assert
            var containErrors = errorNodes.Select(x => x.InnerText).ToList().SequenceEqual(expectedErrorMessages);

            containErrors.Should().BeTrue();

            // Check if response contain cookie with Identity
            const string identityCookieName = ".AspNetCore.Identity.Application";
            var existsCookie = CookiesHelper.ExistsCookie(responseMessage, identityCookieName);

            // Assert Identity cookie
            existsCookie.Should().BeFalse();
        }
    }
}
