using AutoMapper;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public class TenantMapperProfile : Profile
    {
        public TenantMapperProfile()
        {
            // entity to model
            CreateMap<Tenant, CreateTenantDto>(MemberList.Destination);
            CreateMap<Tenant, UpdateTenantDto>(MemberList.Destination);
            CreateMap<Tenant, TenantDto>(MemberList.Destination);

            CreateMap<Edition, CreateEditionDto>(MemberList.Destination);
            CreateMap<Edition, UpdateEditionDto>(MemberList.Destination);
            CreateMap<Edition, EditionDto>(MemberList.Destination);

            //PagedLists
            CreateMap<PagedList<Tenant>, TenantsDto>(MemberList.Destination)
                .ForMember(x => x.Tenants, opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<Edition>, EditionsDto>(MemberList.Destination)
                .ForMember(x => x.Editions, opt => opt.MapFrom(src => src.Data));
        }
    }
}
