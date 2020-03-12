using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Iserv.IdentityServer4.BusinessLogic.Settings;
using Microsoft.Extensions.Logging;

namespace Iserv.IdentityServer4.BusinessLogic.TokenValidators
{
    public class VkTokenValidator : IVkTokenValidator
    {
        private readonly string _accessTokenEndpoint;
        private const string UserInfoEndpoint = "https://api.vk.com/method/users.get?fields=nickname&access_token={token}&v=5.103";
        private const string AuthCodeReplacement = "{code}";
        private const string AccessTokenReplacement = "{token}";
        private readonly ILogger<VkTokenValidator> _logger;

        public VkTokenValidator(SocialOptions socialOptions, ILogger<VkTokenValidator> logger)
        {
            var vkParams = socialOptions.VkParams;
            _accessTokenEndpoint =
                $"https://oauth.vk.com/access_token?client_id={vkParams.AndroidClientId}&client_secret={vkParams.AndroidClientSecret}&redirect_uri=https://com.mlk/vk?&code={AuthCodeReplacement}";
            _logger = logger;
        }

        public async Task<TokenValidationResult> ValidateAccessTokenAsync(string code, string expectedScope = null)
        {
            if (string.IsNullOrWhiteSpace(code)) return new TokenValidationResult {IsError = true, ErrorDescription = "Код авторизации не указан"};
            using var client = new HttpClient();
            var response = await client.GetAsync(_accessTokenEndpoint.Replace(AuthCodeReplacement, code));
            if (!response.IsSuccessStatusCode)
            {
                const string msg = "Не удалось получить access_token VK";
                _logger.LogWarning(msg + ". " + response.ReasonPhrase);
                return new TokenValidationResult {IsError = true, ErrorDescription = msg};
            }

            var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var email = jsonDocument.RootElement.GetProperty("email").ToString();
            var token = jsonDocument.RootElement.GetProperty("access_token").ToString();
            response = await client.GetAsync(UserInfoEndpoint.Replace(AccessTokenReplacement, token));
            if (!response.IsSuccessStatusCode)
            {
                const string msg = "Не удалось получить данные пользователя VK";
                _logger.LogWarning(msg + ". " + response.ReasonPhrase);
                return new TokenValidationResult {IsError = true, ErrorDescription = msg};
            }

            jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var claimList = new List<Claim> {new Claim("email", email)};
            var item = jsonDocument.RootElement.EnumerateObject().FirstOrDefault();
            if (item.Name == "error")
            {
                _logger.LogWarning(item.Value.GetRawText());
                return new TokenValidationResult {IsError = true, ErrorDescription = item.Value.GetRawText()};
            }

            if (item.Value.ValueKind != JsonValueKind.Array || item.Value.GetArrayLength() < 1)
                return new TokenValidationResult {IsError = true, ErrorDescription = response.ReasonPhrase};
            var props = item.Value[0];
            claimList.Add(new Claim("id", props.GetProperty("id").ToString()));
            claimList.Add(new Claim("FirstName", props.GetProperty("first_name").ToString()));
            claimList.Add(new Claim("LastName", props.GetProperty("last_name").ToString()));
            claimList.Add(new Claim("MiddleName", props.GetProperty("nickname").ToString()));
            return new TokenValidationResult {IsError = false, Claims = claimList};
        }

        public Task<TokenValidationResult> ValidateIdentityTokenAsync(string token, string clientId = null, bool validateLifetime = true)
        {
            throw new NotImplementedException();
        }

        public Task<TokenValidationResult> ValidateRefreshTokenAsync(string token, Client client = null)
        {
            throw new NotImplementedException();
        }
    }
}