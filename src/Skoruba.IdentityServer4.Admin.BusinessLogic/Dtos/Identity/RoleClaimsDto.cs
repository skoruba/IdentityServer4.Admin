using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity
{
    public class RoleClaimsDto : RoleClaimDto
    {
        public RoleClaimsDto()
        {
            Claims = new List<RoleClaimDto>();
        }

        public List<RoleClaimDto> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
