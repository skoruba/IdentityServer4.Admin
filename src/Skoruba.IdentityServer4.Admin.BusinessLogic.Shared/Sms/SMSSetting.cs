namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Sms
{
    /// <summary>
    /// Параметры отправки смс
    /// </summary>
    public class SMSSetting
    {
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
