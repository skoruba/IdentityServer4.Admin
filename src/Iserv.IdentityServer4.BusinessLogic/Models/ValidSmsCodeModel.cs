using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Модель проверки номера телефона
    /// </summary>
    public class ValidSmsCodeModel
    {
        /// <summary>
        /// Проверяемый номер телефона
        /// </summary>
        [JsonProperty("phone")]
        [FromForm(Name = "phone")]
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Код смс проверяемого номера телефона
        /// </summary>
        [Required]
        [JsonProperty("code")]
        [FromForm(Name = "code")]
        public string SmsCode { get; set; }
    }
}
