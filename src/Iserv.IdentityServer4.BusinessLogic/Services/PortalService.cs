using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Iserv.IdentityServer4.BusinessLogic.Settings;
using Iserv.IdentityServer4.BusinessLogic.ExceptionHandling;
using Iserv.IdentityServer4.BusinessLogic.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    public class PortalService : IPortalService
    {
        public static readonly string PortalCode = "portal";

        private readonly AuthPortalOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PortalService> _logger;

        public PortalService(AuthPortalOptions options, IHttpClientFactory clientFactory, IMemoryCache memoryCache, ILogger<PortalService> logger)
        {
            _options = options;
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        private async Task<string> readResponseAsStringAsync(HttpResponseMessage response)
        {
            var result = await response.Content?.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return "Portal. Unauthorized";
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    if (!string.IsNullOrWhiteSpace(result)) _logger.LogInformation(result);
                    return "Portal. " + result;
                }
                else
                {
                    throw new PortalException(result);
                }
            }
            return result;
        }

        public async Task UpdateSessionAsync()
        {
            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(_options.RootAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _options.Login, _options.Password))));
            await client.GetAsync("");
            var cookie = handler.CookieContainer.GetCookies(new Uri(_options.RootAddress)).FirstOrDefault();
            if (cookie != null)
            {
                _memoryCache.Set(PortalCode, cookie.Value);
            }
        }

        public string GetCookie()
        {
            return _memoryCache.Get(PortalCode)?.ToString();
        }

        public async Task<PortalResult<Guid>> GetUserIdByAuthAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ValidationException("Логин пользователя не указан");
            if (string.IsNullOrWhiteSpace(password))
                throw new ValidationException("Пароль пользователя не указан");
            var client = _clientFactory.CreateClient(PortalCode);
            var response = await client.PostAsync("tehprisEE_auth/signin", new StringContent(JsonConvert.SerializeObject(new { email = userName, password }), Encoding.UTF8));
            var txt = await readResponseAsStringAsync(response);
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PortalAuthResultSuccess>(txt);
                if (result != null)
                    return new PortalResult<Guid>() { Value = result.UserId };
            }
            return new PortalResult<Guid>() { IsError = true, Message = txt };
        }

        public async Task<PortalResult<Dictionary<string, object>>> GetUserAsync(Guid idext)
        {
            if (idext.Equals(Guid.Empty))
                throw new ValidationException("Внешний Id пользователя на портале пустой");
            var client = _clientFactory.CreateClient(PortalCode);
            var response = await client.GetAsync("tehprisEE_profiles/" + idext);
            var txt = await readResponseAsStringAsync(response);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return new PortalResult<Dictionary<string, object>>() { Value = JsonConvert.DeserializeObject<Dictionary<string, object>>(txt) };
                }
                catch
                {
                    new PortalResult<Dictionary<string, object>>() { IsError = true, Message = txt };
                }
            }
            return new PortalResult<Dictionary<string, object>>() { IsError = true, Message = txt };
        }

        public async Task<PortalResult<Guid>> RegistrateAsync(PortalRegistrationData userProfile)
        {
            var client = _clientFactory.CreateClient(PortalCode);
            var response = await client.PostAsync("tehprisEE_auth/register", new StringContent(JsonConvert.SerializeObject(userProfile), Encoding.UTF8));
            var txt = await readResponseAsStringAsync(response);
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PortalAuthResultSuccess>(txt);
                if (result != null)
                    return new PortalResult<Guid>() { Value = result.UserId };
            }
            return new PortalResult<Guid>() { IsError = true, Message = txt };
        }

        public async Task<PortalResult> UpdateUserAsync(Guid idext, Dictionary<string, object> values, IEnumerable<FileModel> files = null)
        {
            var client = _clientFactory.CreateClient(PortalCode);
            var multiForm = new MultipartFormDataContent();
            multiForm.Add(new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8), "attributes");
            foreach (var file in files)
            {
                var imageContent = new ByteArrayContent(file.FileData);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                multiForm.Add(imageContent, file.Tag, file.Name);
            }
        
            var response = await client.PostAsync("tehprisEE_profiles/" + idext, multiForm);
            var txt = await readResponseAsStringAsync(response);
            if (response.IsSuccessStatusCode)
            {
                return new PortalResult() { Message = txt };
            }
            return new PortalResult() { IsError = true, Message = txt };
        }

        ///// <summary>
        ///// Изменение пароля профиля
        ///// </summary>
        ///// <param name="extProfileId">Внешний идентификатор профиля</param>
        ///// <param name="password">Новый пароль профиля</param>
        ///// <returns></returns>
        //public async Task<ResponseResult<string>> SetPasswordAsync(Guid extProfileId, string password)
        //{
        //    var request = new RestRequest("tehprisEE_profiles/{userid}", Method.POST);
        //    request.AddParameter(new Parameter("userid", extProfileId, ParameterType.UrlSegment));
        //    request.AlwaysMultipartFormData = true;
        //    request.AddHeader("cache-control", "no-cache");
        //    request.AddParameter("attributes\"\r\nContent-Type: text/plain; charset=UTF-8\"", $@"{{ ""password"": ""{ password }"" }}", ParameterType.GetOrPost);

        //    var response = await _requestSender.SendRequestAsync(request);
        //    var result = new ResponseResult<string> { HttpStatusCode = response.StatusCode, Content = response.Content };

        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {
        //        result.Value = response.Content;
        //        return result;
        //    }

        //    result.IsError = true;
        //    result.Message = result.Content;
        //    return result;
        //}
    }
}
