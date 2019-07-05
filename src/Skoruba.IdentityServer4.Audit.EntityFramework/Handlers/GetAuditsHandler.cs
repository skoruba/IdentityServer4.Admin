using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Audit.Core;
using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Query;
using Skoruba.IdentityServer4.Audit.EntityFramework.MediatR;

namespace Skoruba.IdentityServer4.Audit.EntityFramework.Handlers
{
    public class GetAuditsHandler : RequestHandler<GetAudits, AuditsDto>
    {
        public GetAuditsHandler(AuditDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public override async Task<AuditsDto> Handle(RequestWrapper<GetAudits, AuditsDto> request, CancellationToken cancellationToken)
        {
            var audits = await DbContext.Audits
                .ProjectTo<AuditDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new AuditsDto();
        }
    }
}