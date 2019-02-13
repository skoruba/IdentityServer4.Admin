using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class UserProvidersDto<TUserDtoKey> : UserProviderDto<TUserDtoKey>
    {
        public UserProvidersDto()
        {
            Providers = new List<UserProviderDto<TUserDtoKey>>();
        }

        public List<UserProviderDto<TUserDtoKey>> Providers { get; set; }
    }
}
