using AutoMapper;
using Skoruba.IdentityServer4.Admin.Api.Dtos.Users;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.Api.Mappers
{
    public class IdentityMapperProfile<TUserDtoKey, TUserClaimsDto, TUserClaimDto> : Profile
        where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
        where TUserClaimDto : UserClaimDto<TUserDtoKey>
    {
        public IdentityMapperProfile()
        {
            // entity to model
            CreateMap<TUserClaimsDto, UserClaimsApiDto<TUserDtoKey>>(MemberList.Destination);

            CreateMap<TUserClaimDto, UserClaimApiDto<TUserDtoKey>>(MemberList.Destination);
        }
    }
}