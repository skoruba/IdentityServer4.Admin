using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class IdentityResourceDto
	{
		public IdentityResourceDto()
		{
			UserClaims = new List<string>();
			Properties = new List<IdentityResourcePropertyDto>();
		}

		public int Id { get; set; }

        [Required]
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool Enabled { get; set; } = true;

		public bool ShowInDiscoveryDocument { get; set; } = true;
		
		public bool Required { get; set; }

		public bool Emphasize { get; set; }

		public List<string> UserClaims { get; set; }

		public string UserClaimsItems { get; set; }

		public DateTime Created { get; set; }

		public DateTime? Updated { get; set; }

		public List<IdentityResourcePropertyDto> Properties { get; set; }

		public bool NonEditable { get; set; }
	}
}