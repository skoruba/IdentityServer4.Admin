using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity
{
    public class RoleClaimsDto<TRoleDtoKey, TClaimDtoKey> : RoleClaimDto<TRoleDtoKey, TClaimDtoKey>
    {
        public RoleClaimsDto()
        {
            Claims = new List<RoleClaimDto<TRoleDtoKey, TClaimDtoKey>>();
        }

        public List<RoleClaimDto<TRoleDtoKey, TClaimDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
