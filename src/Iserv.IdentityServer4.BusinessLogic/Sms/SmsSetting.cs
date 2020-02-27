﻿ namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    /// <summary>
    /// Параметры отправки смс
    /// </summary>
    public class SmsSetting
    {
        /// <summary>
        /// Поставщик сервиса смс
        /// </summary>
        public SmsProvider Provider { get; set; }
        
        /// <summary>
        /// Логин аккаунта
        /// </summary>
        public string Login { get; set; }
        
        /// <summary>
        /// Пароль аккаунта
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Корневой Url сервиса
        /// </summary>
        public string RootUrl { get; set; }
        
        /// <summary>
        /// Идентификатор аккаунта
        /// </summary>
        public string AccountSid { get; set; }

        /// <summary>
        /// Токен авторизации
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Номер телефона отправителя по умолчанию
        /// </summary>
        public string DefaultPhoneFrom { get; set; }
    }
}
