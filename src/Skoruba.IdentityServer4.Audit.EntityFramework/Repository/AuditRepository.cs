using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Extensions;
using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Entities;
using Skoruba.IdentityServer4.Audit.Core.Interfaces;
using Skoruba.IdentityServer4.Audit.Core.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.EntityFramework.Repository
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AuditDbContext _dbContext;

        public AuditRepository(AuditDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<AuditEntity>> GetAuditsAsync(GetAudits query)
        {
            var pagedList = new PagedList<AuditEntity>();

            var queryable = _dbContext.Audits.AsQueryable();

            if (query.FromDate.HasValue)
                queryable = queryable.Where(a => a.TimeStamp >= query.FromDate.Value.UtcDateTime);

            if (query.ToDate.HasValue)
                queryable = queryable.Where(a => a.TimeStamp <= query.ToDate.Value.UtcDateTime);

            if (!string.IsNullOrWhiteSpace(query.SubjectId))
                queryable = queryable.Where(a => a.SubjectId == query.SubjectId);

            var audits = await queryable.PageBy(a => a.Id, query.Page, query.PageSize).ToListAsync();
            pagedList.Data.AddRange(audits);
            pagedList.TotalCount = await queryable.CountAsync();
            pagedList.PageSize = query.PageSize;

            return pagedList;
        }
    }
}