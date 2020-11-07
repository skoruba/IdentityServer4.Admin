using AutoMapper;
using Skoruba.IdentityServer4.Admin.Api.Dtos.ApiResources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.Api.Mappers
{
    public class ApiScopeApiMapperProfile : Profile
    {
        public ApiScopeApiMapperProfile()
        {
            // Api Scopes
            CreateMap<ApiScopesDto, ApiScopesApiDto>(MemberList.Destination)
                .ReverseMap();

            CreateMap<ApiScopeDto, ApiScopeApiDto>(MemberList.Destination)
                .ReverseMap();

            CreateMap<ApiScopesDto, ApiScopeApiDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApiScopeId))
                .ReverseMap();
        }
    }
}