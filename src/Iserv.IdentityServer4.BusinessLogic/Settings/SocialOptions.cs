namespace Iserv.IdentityServer4.BusinessLogic.Settings
{
    /// <summary>
    /// Параметры социальной сети
    /// </summary>
    public class SocialParams
    {
        /// <summary>
        /// Id Web приложения
        /// </summary>
        public string WebClientId { get; set; }
        
        /// <summary>
        /// Защищенный ключ Web приложения
        /// </summary>
        public string WebClientSecret { get; set; }
        
        /// <summary>
        /// Id Android приложения
        /// </summary>
        public string AndroidClientId { get; set; }
        
        /// <summary>
        /// Защищенный ключ Web приложения
        /// </summary>
        public string AndroidClientSecret { get; set; }
        
        /// <summary>
        /// Id IOS приложения
        /// </summary>
        public string IosClientId { get; set; }
        
        /// <summary>
        /// Защищенный ключ Web приложения
        /// </summary>
        public string IosClientSecret { get; set; }
    }
    
    /// <summary>
    /// Настройки социальных сетей
    /// </summary>
    public class SocialOptions
    {
        /// <summary>
        /// Параметры Google
        /// </summary>
        public SocialParams GoogleParams { get; set; }
        
        /// <summary>
        /// Параметры Yandex
        /// </summary>
        public SocialParams YandexParams { get; set; }
        
        /// <summary>
        /// Параметры Facebook
        /// </summary>
        public SocialParams FacebookParams { get; set; }
        
        /// <summary>
        /// Параметры Vk
        /// </summary>
        public SocialParams VkParams { get; set; }
    }
}
