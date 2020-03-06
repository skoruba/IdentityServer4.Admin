using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Iserv.IdentityServer4.BusinessLogic.Sms.ExceptionHandling;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    public class SmsSenderDevino : ISmsService
    {
        private readonly SmsSetting _smsSetting;
        private readonly ILogger<SmsSenderDevino> _logger;
        private static string _sessionId = null;
        private static Timer _timerRefreshSessionId;

        public SmsSenderDevino(SmsSetting smsSetting, ILogger<SmsSenderDevino> logger)
        {
            _smsSetting = smsSetting;
            _logger = logger;
            if (_timerRefreshSessionId != null) return;
            _timerRefreshSessionId = new Timer {Interval = 5000000};
            _timerRefreshSessionId.Elapsed += timerRefreshSessionId_Elapsed;
            _timerRefreshSessionId.Enabled = true;
            RefreshSessionId();
        }
        
        private void RefreshSessionId()
        {
            _sessionId = null;
            var result = GetSessionIdAsync().Result;
            if (!result.IsError)
            {
                _sessionId = result.Message;
                return;
            }
            _logger.LogError(result.Message);
            _sessionId = null;
        } 

        private void timerRefreshSessionId_Elapsed(object sender, EventArgs e)
        {
            RefreshSessionId();
        }

        private async Task<string> ReadResponseAsStringAsync(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) return result;
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return "Запрос не авторизованный";
                case HttpStatusCode.BadRequest:
                {
                    if (!string.IsNullOrWhiteSpace(result)) _logger.LogError(result);
                    return result;
                }
                default:
                    throw new DevinoException(result);
            }
        }

        /// <summary>
        /// Получить идентификатор сессии
        /// </summary>
        /// <param name="login">Логин клиента</param>
        /// <param name="password">Пароль клиента</param>
        /// <returns>Идентификатор сессии</returns>
        private async Task<SmsResult> GetSessionIdAsync()
        {
            if (_sessionId != null) return new SmsResult {Message = _sessionId};
            try
            {
                var values = HttpUtility.ParseQueryString(string.Empty);
                values["login"] = _smsSetting.Login;
                values["password"] = _smsSetting.Password;
                var requestUrl = "user/sessionid" + "?" + values;
                var client = new HttpClient() {BaseAddress = new Uri(_smsSetting.RootUrl)};
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                var response = await client.GetAsync(requestUrl);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode) return new SmsResult() {Message = result.Substring(1, result.Length - 2)};
                _logger.LogError(result);
                return new SmsResult {IsError = true, Message = "Не удалось получить идентификатор сессии отправки смс"};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new SmsResult {IsError = true, Message = "Не удалось получить идентификатор сессии отправки смс"};
            }
        }

        private async Task<SmsResult> SendAsync(bool isPost, string path, NameValueCollection values)
        {
            var result = await GetSessionIdAsync();
            if (result.IsError)
            {
                throw new DevinoException(result.Message);
            }

            values["sessionId"] = result.Message;
            var requestUrl = path + "?" + values.ToString();
            var client = new HttpClient() {BaseAddress = new Uri(_smsSetting.RootUrl)};
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var response = isPost ? await client.PostAsync(requestUrl, null) : await client.GetAsync(requestUrl);
            var txt = await ReadResponseAsStringAsync(response);
            return !response.IsSuccessStatusCode ? new SmsResult() {IsError = true, Message = txt} : new SmsResult {Message = txt};
        }

        private async Task<SmsResult> PostAsync(string path, Dictionary<string, object> values)
        {
            var result = await GetSessionIdAsync();
            if (result.IsError)
            {
                throw new DevinoException(result.Message);
            }

            values.Add("sessionId", result.Message);
            var client = new HttpClient() {BaseAddress = new Uri(_smsSetting.RootUrl)};
            client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            var multiForm = new MultipartFormDataContent {new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8)};
            var response = await client.PostAsync(path, multiForm);
            var txt = await ReadResponseAsStringAsync(response);
            return !response.IsSuccessStatusCode ? new SmsResult() {IsError = true, Message = txt} : new SmsResult {Message = txt};
        }

        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="destinationAddress">Мобильный телефонный номер получателя сообщения</param>
        /// <param name="data">Текст сообщения</param>
        /// <param name="sendDate">Дата и время отправки (для моментальной отправки можно не передавать)</param>
        /// <param name="validity">Время жизни сообщения (в минутах)</param>
        /// <returns>Идентификаторы частей сообщения 
        /// (если сообщение больше 70 символов на кириллице или 160 на латинице, оно разбивается на несколько)</returns>
        private async Task<SmsResult> SendMessageAsync(string destinationAddress, string data, DateTime? sendDate = null,
            int validity = 0)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["sourceAddress"] = _smsSetting.SourceAddress;
            queryString["destinationAddress"] = destinationAddress.Replace("+", "");
            queryString["data"] = data;
            queryString["validity"] = validity.ToString();
            if (sendDate.HasValue)
            {
                queryString["sendDate"] = sendDate.Value.ToString("yyyy-MM-ddThh:mm:ss");
            }

            return await SendAsync(true, "Sms/Send", queryString);
        }

        public async Task<SmsResult> SendSmsAsync(string numberTo, string message)
        {
            return await SendSmsAsync(_smsSetting.DefaultPhoneFrom, numberTo, message);
        }

        public async Task<SmsResult> SendSmsAsync(string numberFrom, string numberTo, string message)
        {
            var result = await SendMessageAsync(numberTo, message);
            if (!result.IsError)
            {
                result.Message = "Сообщение отправилось";
            }

            return result;
        }

        public void Dispose()
        {
        }
    }
}