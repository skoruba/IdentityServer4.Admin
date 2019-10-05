using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Enums;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Extensions;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories
{
    public class PersistedGrantRepository<TDbContext> : IPersistedGrantRepository
        where TDbContext : DbContext, IAdminPersistedGrantDbContext
        
    {
        protected readonly TDbContext DbContext;

        public bool AutoSaveChanges { get; set; } = true;

        public PersistedGrantRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<PagedList<PersistedGrantDataView>> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<PersistedGrantDataView>();

            var persistedGrantByUsers = (from pe in DbContext.PersistedGrants
                                         select new PersistedGrantDataView
                                         {
                                             SubjectId = pe.SubjectId,
                                             SubjectName = string.Empty
                                         })
                                        .Distinct();

            Expression<Func<PersistedGrantDataView, bool>> searchCondition = x => x.SubjectId.Contains(search);

            var persistedGrantsData = await persistedGrantByUsers.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.SubjectId, page, pageSize).ToListAsync();
            var persistedGrantsDataCount = await persistedGrantByUsers.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();

            pagedList.Data.AddRange(persistedGrantsData);
            pagedList.TotalCount = persistedGrantsDataCount;
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<PersistedGrant>> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<PersistedGrant>();

            var persistedGrantsData = await DbContext.PersistedGrants.Where(x => x.SubjectId == subjectId).Select(x => new PersistedGrant()
            {
                SubjectId = x.SubjectId,
                Type = x.Type,
                Key = x.Key,
                ClientId = x.ClientId,
                Data = x.Data,
                Expiration = x.Expiration,
                CreationTime = x.CreationTime
            }).PageBy(x => x.SubjectId, page, pageSize).ToListAsync();

            var persistedGrantsCount = await DbContext.PersistedGrants.Where(x => x.SubjectId == subjectId).CountAsync();

            pagedList.Data.AddRange(persistedGrantsData);
            pagedList.TotalCount = persistedGrantsCount;
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<PersistedGrant> GetPersistedGrantAsync(string key)
        {
            return DbContext.PersistedGrants.SingleOrDefaultAsync(x => x.Key == key);
        }

        public virtual async Task<int> DeletePersistedGrantAsync(string key)
        {
            var persistedGrant = await DbContext.PersistedGrants.Where(x => x.Key == key).SingleOrDefaultAsync();

            DbContext.PersistedGrants.Remove(persistedGrant);

            return await AutoSaveChangesAsync();
        }

        public virtual Task<bool> ExistsPersistedGrantsAsync(string subjectId)
        {
            return DbContext.PersistedGrants.AnyAsync(x => x.SubjectId == subjectId);
        }

        public virtual async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            var grants = await DbContext.PersistedGrants.Where(x => x.SubjectId == userId).ToListAsync();

            DbContext.RemoveRange(grants);

            return await AutoSaveChangesAsync();
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }
    }
}