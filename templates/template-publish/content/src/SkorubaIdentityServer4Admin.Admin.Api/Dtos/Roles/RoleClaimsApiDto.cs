using System.Collections.Generic;

namespace SkorubaIdentityServer4Admin.Admin.Api.Dtos.Roles
{
    public class RoleClaimsApiDto<TRoleDtoKey>
    {
        public RoleClaimsApiDto()
        {
            Claims = new List<RoleClaimApiDto<TRoleDtoKey>>();
        }

        public List<RoleClaimApiDto<TRoleDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}





