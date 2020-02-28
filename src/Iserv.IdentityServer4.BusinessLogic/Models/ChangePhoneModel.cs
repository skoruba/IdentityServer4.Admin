using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Модель изменения номера телефона пользователя
    /// </summary>
    public class ChangePhoneModel
    {
        /// <summary>
        /// Новый номер телефона пользователя
        /// </summary>
        [JsonProperty("phone")]
        [FromForm(Name = "phone")]
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Код смс проверяемого номера телефона
        /// </summary>
        [JsonProperty("code")]
        [FromForm(Name = "code")]
        [Required]
        public string SmsCode { get; set; }
    }
}
