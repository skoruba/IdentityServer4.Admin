using AutoMapper;
using Skoruba.IdentityServer4.Admin.Api.Dtos.IdentityResources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.Api.Mappers
{
    public class IdentityResourceApiMapperProfile : Profile
    {
        public IdentityResourceApiMapperProfile()
        {
            CreateMap<IdentityResourcesDto, IdentityResourcesApiDto>(MemberList.Destination)
                .ReverseMap();

            CreateMap<IdentityResourceDto, IdentityResourceApiDto>(MemberList.Destination)
                .ReverseMap();
        }
    }
}