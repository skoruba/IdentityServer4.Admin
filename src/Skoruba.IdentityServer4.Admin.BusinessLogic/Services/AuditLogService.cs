using System.Threading.Tasks;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Log;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class AuditLogService<TAuditLog> : IAuditLogService 
        where TAuditLog : AuditLog
    {
        private readonly IAuditLogRepository<TAuditLog> _auditLogRepository;

        public AuditLogService(IAuditLogRepository<TAuditLog> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }
        
        public async Task<AuditLogsDto> GetAsync(int page = 1, int pageSize = 10)
        {
            var pagedList = await _auditLogRepository.GetAsync(page, pageSize);

            var auditLogsDto = pagedList.ToModel();

            return auditLogsDto;
        }
    }
}
