using System;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.STS.Identity.ViewModels.Manage
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
        [MaxLength(255)]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [MaxLength(255)]
        [Display(Name = "Website")]
        public string Website { get; set; }

        [MaxLength(255)]
        [Display(Name = "Profile")]
        public string Profile { get; set; }

        [MaxLength(255)]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [MaxLength(255)]
        [Display(Name = "City")]
        public string Locality { get; set; }

        [MaxLength(255)]
        [Display(Name = "Region")]
        public string Region { get; set; }

        [MaxLength(255)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [MaxLength(255)]
        [Display(Name = "Country")]
        public string Country { get; set; }
    }
}
