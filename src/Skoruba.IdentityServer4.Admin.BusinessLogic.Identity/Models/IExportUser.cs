using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Models
{
    public interface IExportUser
    {
        [Required]
        [JsonProperty("id")]
        Guid Id { get; set; }

        [Required]
        [JsonProperty("idext")]
        Guid ExtId { get; set; }

        [Phone]
        [JsonProperty("phone_number")]
        string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty("e_mail")]
        string Email { get; set; }

        [Required]
        [JsonProperty("password")]
        string Password { get; set; }

        [Required]
        [JsonProperty("status")]
        string Status { get; set; }
    }
}
