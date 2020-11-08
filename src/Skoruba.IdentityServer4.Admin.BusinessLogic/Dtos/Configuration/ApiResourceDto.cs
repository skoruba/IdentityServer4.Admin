using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiResourceDto
	{
		public ApiResourceDto()
		{
			UserClaims = new List<string>();
			Scopes = new List<string>();
			AllowedAccessTokenSigningAlgorithms = new List<string>();
		}

		public int Id { get; set; }

        [Required]
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool Enabled { get; set; } = true;

		public List<string> UserClaims { get; set; }

		public string UserClaimsItems { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        public List<string> AllowedAccessTokenSigningAlgorithms { get; set; }

        public string AllowedAccessTokenSigningAlgorithmsItems { get; set; }

		public List<string> Scopes { get; set; }

        public string ScopesItems { get; set; }
	}
}