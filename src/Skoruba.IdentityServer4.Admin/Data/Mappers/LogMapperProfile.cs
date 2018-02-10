using AutoMapper;
using Skoruba.IdentityServer4.Admin.Data.Entities;
using Skoruba.IdentityServer4.Admin.ViewModels.Common;
using Skoruba.IdentityServer4.Admin.ViewModels.Log;

namespace Skoruba.IdentityServer4.Admin.Data.Mappers
{
    public class LogMapperProfile : Profile
    {
        public LogMapperProfile()
        {
            CreateMap<Log, LogDto>(MemberList.Destination)
                .ReverseMap();
            
            CreateMap<PagedList<Log>, LogsDto>(MemberList.Destination)
                .ForMember(x => x.Logs, opt => opt.MapFrom(src => src.Data));
        }
    }
}
