using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class IdentityResourceClaimDto
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public string Type { get; set; }

		[Required]
		public int IdentityResourceId { get; set; }

		public virtual IdentityResourceDto IdentityResource { get; set; }
	}
}
