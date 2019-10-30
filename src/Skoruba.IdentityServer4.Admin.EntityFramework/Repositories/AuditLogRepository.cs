using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Extensions;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories
{
    public class AuditLogRepository<TDbContext, TAuditLog> : IAuditLogRepository<TAuditLog>
        where TDbContext : IAuditLoggingDbContext<TAuditLog>
        where TAuditLog : AuditLog
    {
        protected readonly TDbContext DbContext;

        public AuditLogRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<PagedList<TAuditLog>> GetAsync(int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<TAuditLog>();

            var auditLogs = await DbContext.AuditLog
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(auditLogs);
            pagedList.PageSize = pageSize;
            pagedList.TotalCount = await DbContext.AuditLog.CountAsync();


            return pagedList;
        }
    }
}
