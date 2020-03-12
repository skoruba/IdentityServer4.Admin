using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Iserv.IdentityServer4.BusinessLogic.Settings;
using Microsoft.Extensions.Logging;

namespace Iserv.IdentityServer4.BusinessLogic.TokenValidators
{
    public class YandexTokenValidator : IYandexTokenValidator
    {
        private const string UserInfoEndpoint = "https://login.yandex.ru/info";
        private readonly ILogger<YandexTokenValidator> _logger;

        public YandexTokenValidator(SocialOptions socialOptions, ILogger<YandexTokenValidator> logger)
        {
            _logger = logger;
        }

        public async Task<TokenValidationResult> ValidateAccessTokenAsync(string token, string expectedScope = null)
        {
            if (string.IsNullOrWhiteSpace(token)) return new TokenValidationResult {IsError = true, ErrorDescription = "Токен авторизации не указан"};
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
            var response = await client.GetAsync(UserInfoEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                const string msg = "Не удалось получить данные пользователя Yandex";
                _logger.LogWarning(msg + ". " + response.ReasonPhrase);
                return new TokenValidationResult {IsError = true, ErrorDescription = msg};
            }

            var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var claimList = new List<Claim>();
            var props = jsonDocument.RootElement;
            claimList.Add(new Claim("id", props.GetProperty("id").ToString()));
            claimList.Add(new Claim("email", props.GetProperty("default_email").ToString()));
            claimList.Add(new Claim("FirstName", props.GetProperty("first_name").ToString()));
            claimList.Add(new Claim("LastName", props.GetProperty("last_name").ToString()));
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