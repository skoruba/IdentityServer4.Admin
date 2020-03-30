using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.STS.Identity.ViewModels.Account
{
    public class RegisterWithoutUsernameViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}