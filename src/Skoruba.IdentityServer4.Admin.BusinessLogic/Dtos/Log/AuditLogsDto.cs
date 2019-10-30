using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Log
{
    public class AuditLogsDto
    {
        public AuditLogsDto()
        {
            Logs = new List<AuditLogDto>();
        }


        public List<AuditLogDto> Logs { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
