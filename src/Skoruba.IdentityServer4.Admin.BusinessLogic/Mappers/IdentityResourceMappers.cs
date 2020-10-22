using System.Collections.Generic;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public static class IdentityResourceMappers
    {
        static IdentityResourceMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }


        //ToModel
        public static IdentityResourceDto ToModel(this IdentityResource resource)
        {
            return resource == null ? null : Mapper.Map<IdentityResourceDto>(resource);
        }

        public static IdentityResourcesDto ToModel(this PagedList<IdentityResource> resource)
        {
            return resource == null ? null : Mapper.Map<IdentityResourcesDto>(resource);
        }

        public static List<IdentityResourceDto> ToModel(this List<IdentityResource> resource)
        {
            return resource == null ? null : Mapper.Map<List<IdentityResourceDto>>(resource);
        }

        //ToEntity
        public static IdentityResource ToEntity(this IdentityResourceDto resource)
        {
            return resource == null ? null : Mapper.Map<IdentityResource>(resource);
        }

        public static List<IdentityResource> ToEntity(this List<IdentityResourceDto> resource)
        {
            return resource == null ? null : Mapper.Map<List<IdentityResource>>(resource);
        }

        //-----------------------------------IdentityResourceProperties---------------------------------------------------
        public static IdentityResourceProperty ToEntity(this IdentityResourcePropertiesDto identityResourceProperties)
        {
            return Mapper.Map<IdentityResourceProperty>(identityResourceProperties);
        }

        public static IdentityResourcePropertiesDto ToModel(this IdentityResourceProperty identityResourceProperty)
        {
            return Mapper.Map<IdentityResourcePropertiesDto>(identityResourceProperty);
        }

        public static IdentityResourcePropertiesDto ToModel(this PagedList<IdentityResourceProperty> identityResourceProperties)
        {
            return Mapper.Map<IdentityResourcePropertiesDto>(identityResourceProperties);
        }

        //--------------------------------------IdentityResourceClaims------------------------------------------------
        public static IdentityResourceClaim ToEntity(this IdentityResourceClaimsDto identityResourceClaims)
        {
            return Mapper.Map<IdentityResourceClaim>(identityResourceClaims);
        }
        public static IdentityResourceClaimsDto ToModel(this IdentityResourceClaim identityResourceClaim)
        {
            return Mapper.Map<IdentityResourceClaimsDto>(identityResourceClaim);
        }

        public static IdentityResourceClaimsDto ToModel(this PagedList<IdentityResourceClaim> identityResourceClaims)
        {
            return Mapper.Map<IdentityResourceClaimsDto>(identityResourceClaims);
        }
    }
}