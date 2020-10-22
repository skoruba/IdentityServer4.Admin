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
    public class ApiResourceMapperProfile : Profile
    {
        public ApiResourceMapperProfile()
        {
            // entity to model
            CreateMap<ApiResource, ApiResourceDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            CreateMap<ApiScope, ApiScopesDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)))
                //.ForMember(x => x.ApiResourceId, opt => opt.MapFrom(src => src.ApiResource.Id))
                .ForMember(x => x.ApiScopeId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ApiScope, ApiScopeDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            CreateMap<ApiResourceSecret, ApiResourceSecretsDto>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ForMember(x => x.ApiSecretId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(x => x.ApiResource.Id));

            CreateMap<ApiResourceSecret, ApiResourceSecretDto>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null));

            CreateMap<ApiResourceProperty, ApiResourcePropertyDto>(MemberList.Destination)
                .ReverseMap();

            CreateMap<ApiResourceProperty, ApiResourcePropertiesDto>(MemberList.Destination)
                .ForMember(dest => dest.Key, opt => opt.Condition(srs => srs != null))
                .ForMember(x => x.ApiResourcePropertyId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(x => x.ApiResource.Id));

			CreateMap<ApiResourceScope, ApiResourceScopeDto>(MemberList.Destination)
				.ForMember(dest => dest.Id, opt => opt.Condition(srs => srs != null))
				.ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
				.ForMember(x => x.Scope, opt => opt.MapFrom(x => x.Scope))
				.ForMember(x => x.ApiResourceId, opt => opt.MapFrom(x => x.ApiResource.Id));

			//PagedLists
			CreateMap<PagedList<ApiResourceScope>, ApiResourceScopesDto>(MemberList.Destination)
			  .ForMember(x => x.ApiResourceScopes, opt => opt.MapFrom(src => src.Data));

			CreateMap<PagedList<ApiResource>, ApiResourcesDto>(MemberList.Destination)
                .ForMember(x => x.ApiResources, opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<ApiScope>, ApiScopesDto>(MemberList.Destination)
                .ForMember(x => x.Scopes, opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<ApiResourceSecret>, ApiResourceSecretsDto>(MemberList.Destination)
                .ForMember(x => x.ApiResourceSecrets, opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<ApiResourceProperty>, ApiResourcePropertiesDto>(MemberList.Destination)
                .ForMember(x => x.ApiResourceProperties, opt => opt.MapFrom(src => src.Data));

            // model to entity
            CreateMap<ApiResourceDto, ApiResource>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiResourceClaim { Type = x })));

            CreateMap<ApiResourceSecretsDto, ApiResourceSecret>(MemberList.Source)
                .ForMember(x => x.ApiResource, opts => opts.MapFrom(src => new ApiResource() { Id = src.ApiResourceId }))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(src => src.ApiResourceId))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ApiSecretId));

            CreateMap<ApiResourcePropertiesDto, ApiResourceProperty>(MemberList.Source)
                .ForMember(x => x.ApiResource, dto => dto.MapFrom(src => new ApiResource() { Id = src.ApiResourceId }))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ApiResourcePropertyId));

            CreateMap<ApiResourceScopeDto, ApiResourceScope>(MemberList.Source)
                .ForMember(x => x.ApiResource, opts => opts.MapFrom(src => new ApiResource() { Id = src.ApiResourceId }))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(src => src.ApiResourceId))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<ApiScopesDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ApiScopeId));

            CreateMap<ApiScopeDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })));
        }
    }
}
