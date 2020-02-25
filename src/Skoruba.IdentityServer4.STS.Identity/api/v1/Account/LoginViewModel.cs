// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.STS.Identity.api.v1.Account
{
	public class LoginViewModel
	{
		[Required]
		public string User { get; set; }
		[Required]
		public string Password { get; set; }

		public bool RememberLogin { get; set; }
	}
}
