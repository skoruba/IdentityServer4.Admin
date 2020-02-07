using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Sms;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Services
{
    /// <summary>
    /// Сервис подтверждения по смс
    /// </summary>
    public class ConfirmBySMSService : IConfirmBySMSService
    {
        private readonly TimeSpan _timeSpan;
        private readonly IMemoryCache _memoryCache;
        private readonly ISMSSender _smsSender;

        public ConfirmBySMSService(TimeSpan timeSpan, IMemoryCache memoryCache, ISMSSender smsSender)
        {
            _timeSpan = timeSpan;
            _memoryCache = memoryCache;
            _smsSender = smsSender;
        }

        private void validPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ValidationException("Phone is null");
            var vPhone = new PhoneAttribute();
            if (!vPhone.IsValid(phone))
                throw new ValidationException(vPhone.ErrorMessage);
        }

        private void validActionName(string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ValidationException("Action name is null");
        }

        private string getCacheKey(string phone, string actionName)
        {
            return $"{phone}_{actionName}";
        }

        /// <summary>
        /// Запрос на подтверждение действия по смс
        /// </summary>
        /// <param name="phone">Номер телефона отправления кода подтверждения</param>
        /// <param name="actionName">Наименование действия</param>
        /// <param name="templateMessage">Шаблон сообщения</param>
        /// <returns>Результат запроса на подтверждение действия по смс</returns>
        public async Task ConfirmAsync(string phone, string actionName, string templateMessage)
        {
            validPhoneNumber(phone);
            validActionName(actionName);
            var code = (new Random()).Next(999999).ToString("D6");
            var msg = await _smsSender.SendSMSAsync(phone, string.Format(templateMessage, code));
            var cacheKey = getCacheKey(phone, actionName);
            if (string.IsNullOrEmpty(msg))
            {
                _memoryCache.Set(cacheKey, code, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_timeSpan));
            }
            else
            {
                _memoryCache.Remove(cacheKey);
                throw new ValidationException(msg);
            }
        }

        /// <summary>
        /// Проверка кода подтверждения действия по смс
        /// </summary>
        /// <param name="phone">Номер телефона проверки кода подтверждения</param>
        /// <param name="actionName">Наименование действия</param>
        /// <param name="code">Код смс подтверждения действия</param>
        /// <returns>Результат проверки кода подтверждения действия</returns>
        public void ValidCode(string phone, string actionName, string code)
        {
            validPhoneNumber(phone);
            validActionName(actionName);
            var codeCache = _memoryCache.Get(getCacheKey(phone, actionName))?.ToString();
            if (codeCache != code)
                throw new ValidationException("Код подтверждения не верный");
        }
    }
}