using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity
{
    public class UserClaimsDto : UserClaimDto
    {
        public UserClaimsDto()
        {
            Claims = new List<UserClaimDto>();
        }
        
        public List<UserClaimDto> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
