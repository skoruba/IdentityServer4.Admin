using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Mocks
{
    public static class UserMocks
    {
        public static string UserPassword = "Pa$$word123";
        public static string AntiForgeryTokenKey = "__RequestVerificationToken";

        public static Dictionary<string, string> GenerateRegisterData()
        {
            return new Dictionary<string, string>
            {
                { "UserName", Guid.NewGuid().ToString()},
                { "Password", UserPassword },
                { "ConfirmPassword", UserPassword},
                { "Email", $"{Guid.NewGuid().ToString()}@{Guid.NewGuid().ToString()}.com"}
            };
        }

        public static Dictionary<string, string> GenerateLoginData(string userName, string password, string antiForgeryToken)
        {
            var loginDataForm = new Dictionary<string, string>
            {
                {"Username", userName},
                {"Password", password},
                {"button", "login"},
                {AntiForgeryTokenKey, antiForgeryToken}
            };

            return loginDataForm;
        }

        public static Dictionary<string, string> GenerateManageProfileData(string email, string antiForgeryToken)
        {
            var manageData = new Dictionary<string, string>
            {
                { "Name", Guid.NewGuid().ToString()},
                { AntiForgeryTokenKey, antiForgeryToken},
                { "Email", email }
            };

            return manageData;
        }

        public static async Task<HttpResponseMessage> RegisterNewUserAsync(HttpClient client, Dictionary<string, string> registerDataForm)
        {
            const string accountRegisterAction = "/Account/Register";

            var registerResponse = await client.GetAsync(accountRegisterAction);
            var antiForgeryToken = await registerResponse.ExtractAntiForgeryToken();

            registerDataForm.Remove(AntiForgeryTokenKey);
            registerDataForm.Add(AntiForgeryTokenKey, antiForgeryToken);

            var requestMessage = RequestHelper.CreatePostRequestWithCookies(accountRegisterAction, registerDataForm, registerResponse);
            var responseMessage = await client.SendAsync(requestMessage);

            return responseMessage;
        }
    }
}
