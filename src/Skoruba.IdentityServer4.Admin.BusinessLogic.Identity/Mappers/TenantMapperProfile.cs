using AutoMapper;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Tenant;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers
{
    public class TenantMapperProfile : Profile
    {
        public TenantMapperProfile()
        {
            CreateMap<Tenant, TenantDto>();

            CreateMap<PagedList<TenantDto>, TenantsDto>(MemberList.Destination)
                .ForMember(x => x.Tenants,
                    opt => opt.MapFrom(src => src.Data))
                ;
        }
    }
}