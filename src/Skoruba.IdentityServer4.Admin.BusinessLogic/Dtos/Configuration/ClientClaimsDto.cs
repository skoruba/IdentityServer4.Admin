using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ClientClaimsDto
	{
		public ClientClaimsDto()
		{
			ClientClaims = new List<ClientClaimDto>();
		}

		public int ClientClaimId { get; set; }

		public int ClientId { get; set; }

	    public string ClientName { get; set; }

        [Required]
		public string Type { get; set; }

	    [Required]
        public string Value { get; set; }

		public List<ClientClaimDto> ClientClaims { get; set; }

		public int TotalCount { get; set; }

		public int PageSize { get; set; }	    
	}
}