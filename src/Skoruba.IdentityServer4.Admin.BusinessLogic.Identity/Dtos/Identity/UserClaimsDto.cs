using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class UserClaimsDto<TKey> : UserClaimDto<TKey>, IUserClaimsDto
    {
        public UserClaimsDto()
        {
            Claims = new List<UserClaimDto<TKey>>();
        }

        public string UserName { get; set; }

        public List<UserClaimDto<TKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        List<IUserClaimDto> IUserClaimsDto.Claims => Claims.Cast<IUserClaimDto>().ToList();
    }
}
