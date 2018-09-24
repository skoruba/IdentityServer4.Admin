using System.Collections.Generic;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;

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

		public static IdentityResource ToEntity(this IdentityResourceDto resource)
		{
			return resource == null ? null : Mapper.Map<IdentityResource>(resource);
		}

		public static List<IdentityResource> ToEntity(this List<IdentityResourceDto> resource)
		{
			return resource == null ? null : Mapper.Map< List<IdentityResource>>(resource);
		}
	}
}