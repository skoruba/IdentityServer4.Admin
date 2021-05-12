using AutoMapper;
using SkorubaIdentityServer4Admin.Admin.Api.Dtos.ApiScopes;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace SkorubaIdentityServer4Admin.Admin.Api.Mappers
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

            // Api Scope Properties
            CreateMap<ApiScopePropertiesDto, ApiScopePropertiesApiDto>(MemberList.Destination)
                .ReverseMap();

            CreateMap<ApiScopePropertyDto, ApiScopePropertyApiDto>(MemberList.Destination)
                .ReverseMap();

            CreateMap<ApiScopePropertiesDto, ApiScopePropertyApiDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApiScopePropertyId))
                .ReverseMap();
        }
    }
}







