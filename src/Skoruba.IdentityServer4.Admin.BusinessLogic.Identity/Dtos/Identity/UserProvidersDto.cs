using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class UserProvidersDto<TKey> : UserProviderDto<TKey>, IUserProvidersDto
    {
        public UserProvidersDto()
        {
            Providers = new List<UserProviderDto<TKey>>();
        }

        public List<UserProviderDto<TKey>> Providers { get; set; }

        List<IUserProviderDto> IUserProvidersDto.Providers => Providers.Cast<IUserProviderDto>().ToList();
    }
}
