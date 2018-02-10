using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.Data.DbContexts;
using Skoruba.IdentityServer4.Admin.Helpers;
using Skoruba.IdentityServer4.Admin.ViewModels.Common;
using Skoruba.IdentityServer4.Admin.ViewModels.Enums;

namespace Skoruba.IdentityServer4.Admin.Data.Repositories
{
    public class IdentityResourceRepository : IIdentityResourceRepository
    {
        private readonly AdminDbContext _dbContext;

        public bool AutoSaveChanges { get; set; } = true;

        public IdentityResourceRepository(AdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<IdentityResource>> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<IdentityResource>();

            Expression<Func<IdentityResource, bool>> searchCondition = x => x.Name.Contains(search);

            var identityResources = await _dbContext.IdentityResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x=> x.Name, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(identityResources);
            pagedList.TotalCount = await _dbContext.IdentityResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<IdentityResource> GetIdentityResourceAsync(int identityResourceId)
        {
            var identityResource = await _dbContext.IdentityResources
                .Include(x => x.UserClaims)
                .Where(x => x.Id == identityResourceId)
                .SingleOrDefaultAsync();

            return identityResource;
        }

        public async Task<int> AddIdentityResourceAsync(IdentityResource identityResource)
        {
            _dbContext.IdentityResources.Add(identityResource);

            return await AutoSaveChangesAsync();
        }

        public async Task<bool> CanInsertIdentityResourceAsync(IdentityResource identityResource)
        {
            if (identityResource.Id == 0)
            {
                var existsWithSameName = await _dbContext.IdentityResources.Where(x => x.Name == identityResource.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await _dbContext.IdentityResources.Where(x => x.Name == identityResource.Name && x.Id != identityResource.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        private async Task RemoveIdentityResourceClaimsAsync(IdentityResource identityResource)
        {
            var identityClaims = await _dbContext.IdentityClaims.Where(x => x.IdentityResource.Id == identityResource.Id).ToListAsync();
            _dbContext.IdentityClaims.RemoveRange(identityClaims);
        }

        public async Task<int> DeleteIdentityResourceAsync(IdentityResource identityResource)
        {
            var identityResourceToDelete = await _dbContext.IdentityResources.Where(x => x.Id == identityResource.Id).SingleOrDefaultAsync();

            _dbContext.IdentityResources.Remove(identityResourceToDelete);
            return await AutoSaveChangesAsync();
        }

        public async Task<int> UpdateIdentityResourceAsync(IdentityResource identityResource)
        {
            //Remove old relations
            await RemoveIdentityResourceClaimsAsync(identityResource);

            //Update with new data
            _dbContext.IdentityResources.Update(identityResource);

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