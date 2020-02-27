using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    public class SmsSenderDevino : ISmsSender
    {
        private readonly SmsSetting _smsSetting;
        
        public SmsSenderDevino(SmsSetting smsSetting)
        {
            _smsSetting = smsSetting;
        }
        
        private async Task<string> GetAsync(string path, string queryString)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(_smsSetting.RootUrl + path);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            var httpWebResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            using (var responseStream = new StreamReader(httpWebResponse.GetResponseStream() ?? throw new WebException(), Encoding.UTF8))
            {
                return await responseStream.ReadToEndAsync();
            }
        }
        
        private async Task<string> PostAsync(string path, string queryString)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(_smsSetting.RootUrl + path);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            var byteArr = Encoding.UTF8.GetBytes(queryString);
            httpWebRequest.ContentLength = byteArr.Length;
            using (var stream = await httpWebRequest.GetRequestStreamAsync())
            {
                stream.Write(byteArr, 0, byteArr.Length);
            }
            var httpWebResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            using (var responseStream = new StreamReader(httpWebResponse.GetResponseStream() ?? throw new WebException(), Encoding.UTF8))
            {
                return await responseStream.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Получить идентификатор сессии
        /// </summary>
        /// <param name="login">Логин клиента</param>
        /// <param name="password">Пароль клиента</param>
        /// <returns>Идентификатор сессии</returns>
        private async Task<string> GetSessionIdAsync(string login, string password)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["login"] = login;
            queryString["password"] = password;
            var result = await GetAsync("/User/SessionId", queryString.ToString());
            return JsonConvert.DeserializeObject<string>(result);
        }
        
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="sessionId">Идентификатор сессии</param>
        /// <param name="sourceAddress">Адрес отправителя сообщения</param>
        /// <param name="destinationAddress">Мобильный телефонный номер получателя сообщения</param>
        /// <param name="data">Текст сообщения</param>
        /// <param name="sendDate">Дата и время отправки (для моментальной отправки можно не передавать)</param>
        /// <param name="validity">Время жизни сообщения (в минутах)</param>
        /// <returns>Идентификаторы частей сообщения 
        /// (если сообщение больше 70 символов на кириллице или 160 на латинице, оно разбивается на несколько)</returns>
        private async Task<List<string>> SendMessageAsync(string sessionId, string sourceAddress, string destinationAddress, string data, DateTime? sendDate = null, int validity = 0)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            queryString["sessionId"] = sessionId;
            queryString["sourceAddress"] = sourceAddress;
            queryString["destinationAddress"] = destinationAddress;
            queryString["data"] = data;
            queryString["validity"] = validity.ToString();

            if (sendDate.HasValue)
            {
                queryString["sendDate"] = sendDate.Value.ToString("yyyy-MM-ddThh:mm:ss");
            }

            var result = await PostAsync("/Sms/Send", queryString.ToString());
            return JsonConvert.DeserializeObject<List<string>>(result);
        }

        public async Task<string> SendSmsAsync(string numberTo, string message)
        {
            return await SendSmsAsync(_smsSetting.DefaultPhoneFrom, numberTo, message);
        }

        public async Task<string> SendSmsAsync(string numberFrom, string numberTo, string message)
        {
            if (string.IsNullOrEmpty(numberFrom) || string.IsNullOrEmpty(numberTo) || string.IsNullOrEmpty(message))
            {
                return null;
            }
            var sessionId = await GetSessionIdAsync(_smsSetting.Login, _smsSetting.Password);
            var result = await SendMessageAsync(sessionId, numberFrom, numberTo, message);
            if (result == null || result.Count == 0) return null;
            var msg = "Failed to send SMS to " + numberTo;
            return result.Where(err => !string.IsNullOrEmpty(err)).Aggregate(msg, (current, err) => current + (". " + err));
        }

        public void Dispose()
        {
        }
    }
}