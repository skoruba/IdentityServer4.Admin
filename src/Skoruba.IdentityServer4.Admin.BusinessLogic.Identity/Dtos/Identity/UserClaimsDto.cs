using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class UserClaimsDto<TUserDtoKey> : UserClaimDto<TUserDtoKey>
    {
        public UserClaimsDto()
        {
            Claims = new List<UserClaimDto<TUserDtoKey>>();
        }

        public string UserName { get; set; }

        public List<UserClaimDto<TUserDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
