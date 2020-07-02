using AutoMapper;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public static class TenantMappers
    {
        internal static IMapper Mapper { get; }

        static TenantMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<TenantMapperProfile>())
                .CreateMapper();
        }

        public static TenantDto ToModel(this Tenant resource)
        {
            return resource == null ? null : Mapper.Map<TenantDto>(resource);
        }

        public static CreateTenantDto ToCreateModel(this Tenant resource)
        {
            return resource == null ? null : Mapper.Map<CreateTenantDto>(resource);
        }

        public static UpdateTenantDto ToUpdateModel(this Tenant resource)
        {
            return resource == null ? null : Mapper.Map<UpdateTenantDto>(resource);
        }

        public static TenantsDto ToModel(this PagedList<Tenant> resources)
        {
            return resources == null ? null : Mapper.Map<TenantsDto>(resources);
        }
    }
}
