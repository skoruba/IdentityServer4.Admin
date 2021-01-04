using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.STS.Identity.MobileAuth.ViewModels
{
    public class PhoneLoginViewModel
    {
        /// <summary>
        /// Unique Device ID (MAC Address / IMEI or etc.)
        /// </summary>
        [Required]
        public string DeviceId { get; set; }


        /// <summary>
        /// FCM ID / Notification ID
        /// </summary>
        public string NotificationId { get; set; }




        [Required]
        [DataType(DataType.PhoneNumber)]
        //[JsonProperty("phone")]
        public string PhoneNumber { get; set; }


        /// <summary>
        /// Mobile Application for SMS OTP Hash Code
        /// </summary>
        [Required]
        public string AppHash { get; set; }
    }
}