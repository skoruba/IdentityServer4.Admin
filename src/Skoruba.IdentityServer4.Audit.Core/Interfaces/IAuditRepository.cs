using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Entities;
using Skoruba.IdentityServer4.Audit.Core.Query;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.Core.Interfaces
{
    public interface IAuditRepository
    {
        Task<PagedList<AuditEntity>> GetAuditsAsync(GetAudits query);
    }
}