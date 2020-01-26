using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Models
{
    public class ExportUser
    {
        [Required]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty("idext")]
        public Guid ExtId { get; set; }

        [Required]
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("login_name_ip")]
        public string LoginNameIp { get; set; }

        [JsonProperty("login_name_ul")]
        public string LoginNameUl { get; set; }

        [Required]
        [JsonProperty("represent_first_name")]
        public string FirstName { get; set; }

        [Required]
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("represent_middle_name")]
        public string MiddleName { get; set; }

        [JsonProperty("gender")]
        public EGenders Gender { get; set; }

        [Phone]
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [Phone]
        [JsonProperty("contactphone")]
        public string ContactPhoneNumber { get; set; } = null;

        [Required]
        [EmailAddress]
        [JsonProperty("e_mail")]
        public string Email { get; set; }

        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }

        [Required]
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("device_id")]
        public Guid? DeviceId { get; set; } = null;

        [JsonProperty("user_type")]
        public string UserType { get; set; } = null;

        [JsonProperty("name_org")]
        public string NameOrg { get; set; } = null;

        [JsonProperty("ogrn_ip")]
        public string OgrnIp { get; set; } = null;

        [JsonProperty("ogrn_ul")]
        public string OgrnUl { get; set; } = null;

        [JsonProperty("opf")]
        public string Opf { get; set; } = null;

        [JsonProperty("sign_notify_active")]
        public bool? SignNotifyActive { get; set; }

        [JsonProperty("sign_push_notify_active")]
        public bool? SignPushNotifyActive { get; set; }

        [JsonProperty("regionaddr_ref")]
        public string RegionAddrRef { get; set; } = null;

        [JsonProperty("user_from_esia")]
        public string UserFromEsia { get; set; } = null;

        [JsonProperty("sign_auto_location_active")]
        public bool? SignAutoLocationActive { get; set; }

        [JsonProperty("address_fias")]
        public string AddressFias { get; set; } = null;

        [JsonProperty("address_yandex")]
        public string AddressYandex { get; set; } = null;

        [JsonProperty("show_svet_attributes")]
        public string ShdShowSvetAttributes { get; set; } = null;

        [JsonProperty("show_extended_attributes")]
        public string ShowExtendedAttributes { get; set; } = null;

        [JsonProperty("state")]
        public string State { get; set; } = null;
    }
}
