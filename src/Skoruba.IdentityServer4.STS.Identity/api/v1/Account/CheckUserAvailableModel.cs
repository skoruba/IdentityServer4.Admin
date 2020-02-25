using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.STS.Identity.api.v1.Account
{
	public class CheckUserAvailableModel
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Email { get; set; }
	}
}
