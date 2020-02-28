using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Модель регистрации нового пользователя
    /// </summary>
    public class RegisterUserModel
    {
        /// <summary>
        /// Email нового пользователя
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Номер телефона нового пользователя
        /// </summary>
        [JsonProperty("phone")]
        [FromForm(Name = "phone")]
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Код смс проверяемого номера телефона
        /// </summary>
        [JsonProperty("code")]
        [FromForm(Name = "code")]
        public string SmsCode { get; set; }
        
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Пароль нового пользователя
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
