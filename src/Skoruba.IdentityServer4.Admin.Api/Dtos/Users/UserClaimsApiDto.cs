using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class UserClaimsApiDto<TUserDtoKey>
    {
        public UserClaimsApiDto()
        {
            Claims = new List<UserClaimApiDto<TUserDtoKey>>();
        }

        public List<UserClaimApiDto<TUserDtoKey>> Claims { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}