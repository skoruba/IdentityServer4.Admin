using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiScopesDto
	{
		public ApiScopesDto()
		{
			Scopes = new List<ApiScopeDto>();
			UserClaims = new List<string>();
		}

		public int ApiResourceId { get; set; }

	    public string ResourceName { get; set; }

		public bool ShowInDiscoveryDocument { get; set; } = true;

		public int ApiScopeId { get; set; }

        [Required]
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool Required { get; set; }

		public bool Emphasize { get; set; }

		public int PageSize { get; set; }

		public int TotalCount { get; set; }

		public List<ApiScopeDto> Scopes { get; set; }

		public List<string> UserClaims { get; set; }

		public string UserClaimsItems { get; set; }
	}
}