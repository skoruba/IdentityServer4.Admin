using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Audit.Core.Dtos
{
    public class AuditsDto
    {
        public AuditsDto()
        {
            Audits = new List<AuditDto>();
        }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<AuditDto> Audits { get; set; }
    }
}