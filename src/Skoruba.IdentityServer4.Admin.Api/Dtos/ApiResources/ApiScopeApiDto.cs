using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.Api.Dtos.ApiResources
{
    public class ApiScopeApiDto
    {
        public ApiScopeApiDto()
        {
            UserClaims = new List<string>();
			Properties = new Dictionary<string, string>();
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

		public bool Enable { get; set; }

		public Dictionary<string, string> Properties { get; set; }
	}
}