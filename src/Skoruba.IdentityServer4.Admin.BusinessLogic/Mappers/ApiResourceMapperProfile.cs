// Based on the IdentityServer4.EntityFramework - authors - Brock Allen & Dominick Baier.
// https://github.com/IdentityServer/IdentityServer4.EntityFramework

// Modified by Jan Škoruba

using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public class ApiResourceMapperProfile : Profile
    {
        public ApiResourceMapperProfile()
        {
            // entity to model
            CreateMap<ApiResource, ApiResourceDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            CreateMap<ApiScope, ApiScopesDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(src => src.ApiResource.Id))
                .ForMember(x => x.ApiScopeId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ApiScope, ApiScopeDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            CreateMap<ApiSecret, ApiSecretsDto>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ForMember(x => x.ApiSecretId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(x => x.ApiResource.Id));

            CreateMap<ApiSecret, ApiSecretDto>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null));

            //PagedLists
            CreateMap<PagedList<ApiResource>, ApiResourcesDto>(MemberList.Destination)
                .ForMember(x => x.ApiResources, opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<ApiScope>, ApiScopesDto>(MemberList.Destination)
                .ForMember(x => x.Scopes, opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<ApiSecret>, ApiSecretsDto>(MemberList.Destination)
                .ForMember(x => x.ApiSecrets, opt => opt.MapFrom(src => src.Data));

            // model to entity
            CreateMap<ApiResourceDto, ApiResource>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiResourceClaim { Type = x })));

            CreateMap<ApiSecretsDto, ApiSecret>(MemberList.Source)
                .ForMember(x => x.ApiResource, opts => opts.MapFrom(src => new ApiResource() {Id = src.ApiResourceId}))
                .ForMember(x=> x.Id, opt => opt.MapFrom(src => src.ApiSecretId));

            CreateMap<ApiScopesDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ApiScopeId));

            CreateMap<ApiScopeDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })));
        }
    }
}
