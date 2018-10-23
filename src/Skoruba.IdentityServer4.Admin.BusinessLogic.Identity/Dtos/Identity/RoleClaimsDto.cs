using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class RoleClaimsDto<TRoleDtoKey> : RoleClaimDto<TRoleDtoKey>
    {
        public RoleClaimsDto()
        {
            Claims = new List<RoleClaimDto<TRoleDtoKey>>();
        }

        public string RoleName { get; set; }

        public List<RoleClaimDto<TRoleDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
