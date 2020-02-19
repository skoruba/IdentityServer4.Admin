﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    public class UserModel
    {
        [Required]
        public Guid Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }

        public Dictionary<string, object> Fields { get; set; }
    }
}
