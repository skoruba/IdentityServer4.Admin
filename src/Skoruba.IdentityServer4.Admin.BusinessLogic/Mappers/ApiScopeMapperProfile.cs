// Based on the IdentityServer4.EntityFramework - authors - Brock Allen & Dominick Baier.
// https://github.com/IdentityServer/IdentityServer4.EntityFramework

// Modified by Jan Škoruba

using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public class ApiScopeMapperProfile : Profile
    {
        public ApiScopeMapperProfile()
        {
            // entity to model
            CreateMap<ApiScope, ApiScopesDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)))
                .ForMember(x => x.ApiScopeId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ApiScope, ApiScopeDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            // PagedLists
            CreateMap<PagedList<ApiScope>, ApiScopesDto>(MemberList.Destination)
                .ForMember(x => x.Scopes, opt => opt.MapFrom(src => src.Data));

            // model to entity
            CreateMap<ApiScopesDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ApiScopeId));

            CreateMap<ApiScopeDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })));
        }
    }
}