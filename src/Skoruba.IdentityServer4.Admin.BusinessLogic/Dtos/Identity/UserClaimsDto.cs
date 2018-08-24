using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity
{
    public class UserClaimsDto<TUserDtoKey, TClaimDtoKey> : UserClaimDto<TUserDtoKey, TClaimDtoKey>
    {
        public UserClaimsDto()
        {
            Claims = new List<UserClaimDto<TUserDtoKey, TClaimDtoKey>>();
        }
        
        public List<UserClaimDto<TUserDtoKey, TClaimDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
