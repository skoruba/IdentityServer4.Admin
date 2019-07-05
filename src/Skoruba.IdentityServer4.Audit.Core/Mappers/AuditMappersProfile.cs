using AutoMapper;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Entities;

namespace Skoruba.IdentityServer4.Audit.Core.Mappers
{
    public class AuditMappersProfile : Profile
    {
        public AuditMappersProfile()
        {
            CreateMap<AuditEntity, AuditDto>();

            //PagedLists
            CreateMap<PagedList<AuditEntity>, AuditsDto>(MemberList.Destination)
                .ForMember(x => x.Audits, opt => opt.MapFrom(src => src.Data));
        }
    }
}