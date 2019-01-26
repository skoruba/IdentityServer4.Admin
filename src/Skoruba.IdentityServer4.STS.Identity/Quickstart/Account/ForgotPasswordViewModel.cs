using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.STS.Identity.Quickstart.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
