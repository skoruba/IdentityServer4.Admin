using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity
{
    public class UserClaimsDto<TUserDtoKey> : UserClaimDto<TUserDtoKey>
    {
        public UserClaimsDto()
        {
            Claims = new List<UserClaimDto<TUserDtoKey>>();
        }
        
        public List<UserClaimDto<TUserDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
