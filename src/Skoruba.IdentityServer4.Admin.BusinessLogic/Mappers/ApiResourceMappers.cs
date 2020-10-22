using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public static class ApiResourceMappers
    {
        static ApiResourceMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        ///---------------------------------------------------------------------------

        public static ApiResourceDto ToModel(this ApiResource resource)
        {
            return resource == null ? null : Mapper.Map<ApiResourceDto>(resource);
        }

        public static ApiResourcesDto ToModel(this PagedList<ApiResource> resources)
        {
            return resources == null ? null : Mapper.Map<ApiResourcesDto>(resources);
        }

        public static ApiResourcePropertiesDto ToModel(this PagedList<ApiResourceProperty> apiResourceProperties)
        {
            return Mapper.Map<ApiResourcePropertiesDto>(apiResourceProperties);
        }

        public static ApiResourcePropertiesDto ToModel(this ApiResourceProperty apiResourceProperty)
        {
            return Mapper.Map<ApiResourcePropertiesDto>(apiResourceProperty);
        }

        ///---------------------------------------------------------------------------
        ///
        public static ApiResourceSecretsDto ToModel(this PagedList<ApiResourceSecret> secrets)
        {
            return secrets == null ? null : Mapper.Map<ApiResourceSecretsDto>(secrets);
        }

        public static ApiResourceSecretsDto ToModel(this ApiResourceSecret resource)
        {
            return resource == null ? null : Mapper.Map<ApiResourceSecretsDto>(resource);
        }

		public static ApiResourceScopesDto ToModel(this PagedList<ApiResourceScope> scopes)
		{
			return scopes == null ? null : Mapper.Map<ApiResourceScopesDto>(scopes);
		}

		public static ApiResourceScopeDto ToModel(this ApiResourceScope resource)
		{
			return resource == null ? null : Mapper.Map<ApiResourceScopeDto>(resource);
		}

        public static ApiResourceScope ToEntity(this ApiResourceScopeDto resourceScope)
        {
            return resourceScope == null ? null : Mapper.Map<ApiResourceScope>(resourceScope);
        }

        public static ApiResource ToEntity(this ApiResourceDto resource)
        {
            return resource == null ? null : Mapper.Map<ApiResource>(resource);
        }

        public static ApiResourceClaim ToEntity(this ApiResourceClaimDto apiResourceClaim)
        {
            return apiResourceClaim == null ? null : Mapper.Map<ApiResourceClaim>(apiResourceClaim);
        }

        public static ApiResourceSecret ToEntity(this ApiSecretsDto resource)
        {
            return resource == null ? null : Mapper.Map<ApiResourceSecret>(resource);
        }

        public static ApiResourceSecret ToEntity(this ApiResourceSecretsDto resource)
        {
            return resource == null ? null : Mapper.Map<ApiResourceSecret>(resource);
        }

        public static ApiResourceProperty ToEntity(this ApiResourcePropertyDto apiResourceProperties)
        {
            return Mapper.Map<ApiResourceProperty>(apiResourceProperties);
        }

        public static ApiResourceProperty ToEntity(this ApiResourcePropertiesDto apiResourceProperties)
        {
            return Mapper.Map<ApiResourceProperty>(apiResourceProperties);
        }
    }
}