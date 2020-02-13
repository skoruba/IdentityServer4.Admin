using System;
namespace Iserv.IdentityServer4.BusinessLogic.Settings
{
    /// <summary>
    /// Шаблоны сообщений
    /// </summary>
    public class MessageTemplates
    {
        /// <summary>
        /// Шаблон сообщения проверки подленности номера телефона пользователю  через смс
        /// </summary>
        public string CheckPhoneNumberSms { get; set; }
        /// <summary>
        /// Шаблон сообщения восстановления пароля пользователя через смс
        /// </summary>
        public string RepairPasswordSms { get; set; }
    }
}
