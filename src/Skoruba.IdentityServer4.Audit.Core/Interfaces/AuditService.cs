using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Mappers;
using Skoruba.IdentityServer4.Audit.Core.Query;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.Core.Interfaces
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository AuditRepository;

        public AuditService(IAuditRepository auditRepository)
        {
            AuditRepository = auditRepository;
        }

        public async Task<AuditsDto> GetAuditsAsync(GetAudits query)
        {
            var pagedList = await AuditRepository.GetAuditsAsync(query);
            var clientsDto = pagedList.ToModel();

            return clientsDto;
        }
    }
}