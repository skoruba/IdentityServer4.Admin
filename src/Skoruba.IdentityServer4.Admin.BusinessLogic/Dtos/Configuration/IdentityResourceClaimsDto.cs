using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
    public class IdentityResourceClaimsDto
    {
		public IdentityResourceClaimsDto()
		{
            IdentityResourceClaims = new List<IdentityResourceClaimDto>();
        }

        public int IdentityResourceId { get; set; }

        public string IdentityResourceName { get; set; }

        public List<IdentityResourceClaimDto> IdentityResourceClaims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
