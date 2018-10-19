using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Enums;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories
{
    public class IdentityResourceRepository<TDbContext> : IIdentityResourceRepository<TDbContext>
        where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        private readonly TDbContext _dbContext;

        public bool AutoSaveChanges { get; set; } = true;

        public IdentityResourceRepository(TDbContext dbContext)
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

        public Task<IdentityResource> GetIdentityResourceAsync(int identityResourceId)
        {
            return _dbContext.IdentityResources
                .Include(x => x.UserClaims)
                .Where(x => x.Id == identityResourceId)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Add new identity resource
        /// </summary>
        /// <param name="identityResource"></param>
        /// <returns>This method return new identity resource id</returns>
        public async Task<int> AddIdentityResourceAsync(IdentityResource identityResource)
        {
            _dbContext.IdentityResources.Add(identityResource);

            await AutoSaveChangesAsync();

            return identityResource.Id;
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