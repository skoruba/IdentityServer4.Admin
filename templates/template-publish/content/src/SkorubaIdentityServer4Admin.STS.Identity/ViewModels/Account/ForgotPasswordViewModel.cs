using SkorubaIdentityServer4Admin.STS.Identity.Configuration;
using System.ComponentModel.DataAnnotations;
using SkorubaIdentityServer4Admin.Shared.Configuration.Identity;

namespace SkorubaIdentityServer4Admin.STS.Identity.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public LoginResolutionPolicy? Policy { get; set; }
        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Username { get; set; }
    }
}






