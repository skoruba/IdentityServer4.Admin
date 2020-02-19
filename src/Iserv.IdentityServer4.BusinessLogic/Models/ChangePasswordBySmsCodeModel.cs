using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Модель изменения пароля через смс код востановления пароля пользователя через телефон
    /// </summary>
    public class ChangePasswordBySmsCodeModel
    {
        /// <summary>
        /// Проверяемый номер телефона
        /// </summary>
        [JsonProperty("phone")]
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Код смс проверяемого номера телефона
        /// </summary>
        [Required]
        [JsonProperty("code")]
        public string SmsCode { get; set; }
        
        /// <summary>
        /// Новый пароль пользователя
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
