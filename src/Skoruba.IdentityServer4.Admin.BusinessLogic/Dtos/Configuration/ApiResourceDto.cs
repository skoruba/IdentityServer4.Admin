using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiResourceDto
	{
		public ApiResourceDto()
		{
			UserClaims = new List<string>();
			Secrets = new List<ApiResourceSecretDto>();
			Scopes = new List<ApiResourceScopeDto>();
			Properties = new List<ApiResourcePropertyDto>();
		}

		public int Id { get; set; }

        [Required]
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool Enabled { get; set; } = true;

		public List<string> UserClaims { get; set; }

		public string UserClaimsItems { get; set; }


		public string AllowedAccessTokenSigningAlgorithms { get; set; }

		public bool ShowInDiscoveryDocument { get; set; }

		public List<ApiResourceSecretDto> Secrets { get; set; }
		
		public List<ApiResourceScopeDto> Scopes { get; set; }

		public List<ApiResourcePropertyDto> Properties { get; set; }

		public DateTime Created { get; set; }

		public DateTime? Updated { get; set; }

		public DateTime? LastAccessed { get; set; }

		public bool NonEditable { get; set; }
	}
}