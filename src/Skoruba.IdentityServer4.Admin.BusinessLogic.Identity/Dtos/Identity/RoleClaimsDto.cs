using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class RoleClaimsDto<TRoleDtoKey> : RoleClaimDto<TRoleDtoKey>, IRoleClaimsDto
    {
        public RoleClaimsDto()
        {
            Claims = new List<RoleClaimDto<TRoleDtoKey>>();
        }

        public string RoleName { get; set; }

        public List<RoleClaimDto<TRoleDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        List<IRoleClaimDto> IRoleClaimsDto.Claims => Claims.Cast<IRoleClaimDto>().ToList();
    }
}
