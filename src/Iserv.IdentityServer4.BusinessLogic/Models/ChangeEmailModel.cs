﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Модель изменения Email пользователя
    /// </summary>
    public class ChangeEmailModel
    {
        /// <summary>
        /// Новый Email пользователя
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Код email проверяемого Email
        /// </summary>
        [JsonProperty("code")]
        [FromForm(Name = "code")]
        [Required]
        public string EmailCode { get; set; }
    }
}