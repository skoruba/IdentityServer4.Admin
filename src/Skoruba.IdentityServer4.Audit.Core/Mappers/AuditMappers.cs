using AutoMapper;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Entities;

namespace Skoruba.IdentityServer4.Audit.Core.Mappers
{
    public static class AuditMappers
    {
        static AuditMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<AuditMappersProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static AuditsDto ToModel(this PagedList<AuditEntity> audits)
        {
            return Mapper.Map<AuditsDto>(audits);
        }
    }

}