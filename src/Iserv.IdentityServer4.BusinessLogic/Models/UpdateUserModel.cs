using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Модель обновляемых данных пользователя
    /// </summary>
    public class UpdateUserModel
    {
        /// <summary>
        /// Id изменяемого пользователя
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Новый Email пользователя
        /// </summary>
        [EmailAddress]
        public string Email { get; set; } = null;

        /// <summary>
        /// Новый номер телефона пользователя
        /// </summary>
        [Phone]
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; } = null;

        /// <summary>
        /// Код смс проверяемого номера телефона
        /// </summary>
        [JsonProperty("code")]
        public string SmsCode { get; set; }

        /// <summary>
        /// Новые данные пользователя
        /// </summary>
        public Dictionary<string, object> Values { get; set; }
        
        /// <summary>
        /// Новые файлы пользователя
        /// </summary>
        public IEnumerable<FileModel> Files { get; set; }
    }
}
