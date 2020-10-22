using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiScopeDto
	{
		public ApiScopeDto()
		{
			ScopeClaims = new List<ApiScopeClaimsDto>();
			ScopeProperties = new List<ApiScopePropetiesDto>();
		}

		public bool ShowInDiscoveryDocument { get; set; } = true;

		public int Id { get; set; }

        [Required]
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool Required { get; set; }

		public bool Emphasize { get; set; }

		public List<string> UserClaims { get; set; }
		
		public List<ApiScopeClaimsDto> ScopeClaims { get; set; }

		public List<ApiScopePropetiesDto> ScopeProperties { get; set; }
	}
}