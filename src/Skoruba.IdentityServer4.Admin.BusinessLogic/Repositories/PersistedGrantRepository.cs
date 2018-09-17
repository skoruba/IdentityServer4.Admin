using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Enums;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories
{
    public class PersistedGrantRepository<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> 
        : IPersistedGrantRepository<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IAdminPersistedGrantIdentityDbContext
        where TUser : IdentityUser<TKey> 
        where TRole : IdentityRole<TKey> 
        where TKey : IEquatable<TKey> 
        where TUserClaim : IdentityUserClaim<TKey> 
        where TUserRole : IdentityUserRole<TKey> 
        where TUserLogin : IdentityUserLogin<TKey> 
        where TRoleClaim : IdentityRoleClaim<TKey> 
        where TUserToken : IdentityUserToken<TKey>
        
    {
        private readonly TDbContext _dbContext;

        public bool AutoSaveChanges { get; set; } = true;

        public PersistedGrantRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<PersistedGrantDataView>> GetPersitedGrantsByUsers(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<PersistedGrantDataView>();

            var persistedGrantByUsers = (from pe in _dbContext.PersistedGrants
                                         join us in _dbContext.Users on pe.SubjectId equals us.Id.ToString() into per
                                         from us in per.DefaultIfEmpty()
                                         select new PersistedGrantDataView
                                         {
                                             SubjectId = pe.SubjectId,
                                             SubjectName = us == null ? string.Empty : us.UserName
                                         })
                                        .Distinct();

            Expression<Func<PersistedGrantDataView, bool>> searchCondition = x => x.SubjectId.Contains(search) || x.SubjectName.Contains(search);

            var persistedGrantsData = await persistedGrantByUsers.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.SubjectId, page, pageSize).ToListAsync();
            var persistedGrantsDataCount = await persistedGrantByUsers.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();

            pagedList.Data.AddRange(persistedGrantsData);
            pagedList.TotalCount = persistedGrantsDataCount;
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<PagedList<PersistedGrant>> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<PersistedGrant>();

            var persistedGrantsData = await _dbContext.PersistedGrants.Where(x => x.SubjectId == subjectId).Select(x => new PersistedGrant()
            {
                SubjectId = x.SubjectId,
                Type = x.Type,
                Key = x.Key,
                ClientId = x.ClientId,
                Data = x.Data,
                Expiration = x.Expiration,
                CreationTime = x.CreationTime
            }).PageBy(x => x.SubjectId, page, pageSize).ToListAsync();

            var persistedGrantsCount = await _dbContext.PersistedGrants.Where(x => x.SubjectId == subjectId).CountAsync();

            pagedList.Data.AddRange(persistedGrantsData);
            pagedList.TotalCount = persistedGrantsCount;
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<PersistedGrant> GetPersitedGrantAsync(string key)
        {
            return _dbContext.PersistedGrants.SingleOrDefaultAsync(x => x.Key == key);
        }

        public async Task<int> DeletePersistedGrantAsync(string key)
        {
            var persistedGrant = await _dbContext.PersistedGrants.Where(x => x.Key == key).SingleOrDefaultAsync();

            _dbContext.PersistedGrants.Remove(persistedGrant);

            return await AutoSaveChangesAsync();
        }

        public Task<bool> ExistsPersistedGrantsAsync(string subjectId)
        {
            return _dbContext.PersistedGrants.AnyAsync(x => x.SubjectId == subjectId);
        }

        public async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            var grants = await _dbContext.PersistedGrants.Where(x => x.SubjectId == userId).ToListAsync();

            _dbContext.RemoveRange(grants);

            return await AutoSaveChangesAsync();
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await _dbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public async Task<int> SaveAllChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}