using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class UserProvidersDto<TUserProviderDto, TKey> : UserProviderDto<TKey>, IUserProvidersDto
        where TUserProviderDto : UserProviderDto<TKey>
    {
        public UserProvidersDto()
        {
            Providers = new List<TUserProviderDto>();
        }

        public List<TUserProviderDto> Providers { get; set; }

        List<IUserProviderDto> IUserProvidersDto.Providers => Providers.Cast<IUserProviderDto>().ToList();
    }
}
