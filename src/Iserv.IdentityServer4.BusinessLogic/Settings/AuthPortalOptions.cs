namespace Iserv.IdentityServer4.BusinessLogic.Settings
{
    /// <summary>
    /// Настройки партала
    /// </summary>
    public class AuthPortalOptions
    {
        /// <summary>
        /// Корневой адрес к сервисам портала
        /// </summary>
        public string RootAddress { get; set; }
        
        /// <summary>
        /// Логин получения интеграционного токена портала
        /// </summary>
        public string Login { get; set; }
        
        /// <summary>
        /// Пароль получения интеграционного токена портала
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Интервал обновления интеграционного токена портала
        /// </summary>
        public string Interval { get; set; }
        
        /// <summary>
        /// Игнорирование преоборазования json формата CamelCase
        /// </summary>
        public bool IgnoreCamelCaseForExtraProp { get; set; }
    }
}
