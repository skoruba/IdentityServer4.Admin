using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Iserv.IdentityServer4.BusinessLogic.Sms;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    /// <summary>
    /// Сервис подтверждения действий
    /// </summary>
    public class ConfirmService : IConfirmService
    {
        private readonly TimeSpan _timeSpan;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public ConfirmService(TimeSpan timeSpan, IMemoryCache memoryCache, IEmailSender emailSender, ISmsSender smsSender)
        {
            _timeSpan = timeSpan;
            _memoryCache = memoryCache;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        private void ValidActionName(string actionName)
        {
            if (actionName == null) throw new ArgumentNullException(nameof(actionName));
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ValidationException("Action name is null");
        }

        private static void ValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is null");
            var vEmail = new EmailAddressAttribute();
            if (!vEmail.IsValid(email))
                throw new ValidationException($"Invalid email. {string.Format(vEmail.ErrorMessage, email)}");
        }

        private static void ValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ValidationException("Phone number is null");
            var vPhone = new PhoneAttribute();
            if (!vPhone.IsValid(phone))
                throw new ValidationException($"Invalid phone number. {string.Format(vPhone.ErrorMessage, phone)}");
        }

        private static string GetCacheKey(string path, string actionName)
        {
            return $"{path}_{actionName}";
        }

        public async Task ConfirmEmailAsync(string email, string actionName, string title, string templateMessage)
        {
            ValidActionName(actionName);
            ValidEmail(email);
            var code = (new Random()).Next(999999).ToString("D6");
            await _emailSender.SendEmailAsync(email, title, string.Format(templateMessage, code));
            var cacheKey = GetCacheKey(email, actionName);
            _memoryCache.Set(cacheKey, code, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_timeSpan));
        }

        public void ValidEmailCode(string email, string actionName, string code)
        {
            ValidActionName(actionName);
            ValidEmail(email);
            var codeCache = _memoryCache.Get(GetCacheKey(email, actionName))?.ToString();
            if (codeCache == null || codeCache != code)
                throw new ValidationException("Код подтверждения не верный");
        }

        public async Task ConfirmSmsAsync(string phone, string actionName, string templateMessage)
        {
            ValidActionName(actionName);
            ValidPhoneNumber(phone);
            var code = (new Random()).Next(999999).ToString("D6");
            var msg = await _smsSender.SendSmsAsync(phone, string.Format(templateMessage, code));
            var cacheKey = GetCacheKey(phone, actionName);
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

        public void ValidSmsCode(string phone, string actionName, string code)
        {
            ValidActionName(actionName);
            ValidPhoneNumber(phone);
            var codeCache = _memoryCache.Get(GetCacheKey(phone, actionName))?.ToString();
            if (codeCache == null || codeCache != code)
                throw new ValidationException("Код подтверждения не верный");
        }
    }
}