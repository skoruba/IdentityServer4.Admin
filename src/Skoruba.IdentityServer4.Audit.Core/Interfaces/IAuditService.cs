using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.Core.Interfaces
{
    public interface IAuditService
    {
        Task<AuditsDto> GetAuditsAsync(GetAudits query);
    }
}