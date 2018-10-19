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
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories
{
    public class ApiResourceRepository<TDbContext> : IApiResourceRepository<TDbContext>
        where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        private readonly TDbContext _dbContext;

        public bool AutoSaveChanges { get; set; } = true;

        public ApiResourceRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<ApiResource>> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiResource>();
            Expression<Func<ApiResource, bool>> searchCondition = x => x.Name.Contains(search);

            var apiResources = await _dbContext.ApiResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Name, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(apiResources);
            pagedList.TotalCount = await _dbContext.ApiResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<ApiResource> GetApiResourceAsync(int apiResourceId)
        {
            return _dbContext.ApiResources
                .Include(x => x.UserClaims)
                .Where(x => x.Id == apiResourceId)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> CanInsertApiResourceAsync(ApiResource apiResource)
        {
            if (apiResource.Id == 0)
            {
                var existsWithSameName = await _dbContext.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await _dbContext.ApiResources.Where(x => x.Name == apiResource.Name && x.Id != apiResource.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        public async Task<bool> CanInsertApiScopeAsync(ApiScope apiScope)
        {
            if (apiScope.Id == 0)
            {
                var existsWithSameName = await _dbContext.ApiScopes.Where(x => x.Name == apiScope.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await _dbContext.ApiScopes.Where(x => x.Name == apiScope.Name && x.Id != apiScope.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        /// <summary>
        /// Add new api resource
        /// </summary>
        /// <param name="apiResource"></param>
        /// <returns>This method return new api resource id</returns>
        public async Task<int> AddApiResourceAsync(ApiResource apiResource)
        {
            _dbContext.ApiResources.Add(apiResource);

            await AutoSaveChangesAsync();

            return apiResource.Id;
        }

        private async Task RemoveApiResourceClaimsAsync(ApiResource identityResource)
        {
            //Remove old identity claims
            var apiResourceClaims = await _dbContext.ApiResourceClaims.Where(x => x.ApiResource.Id == identityResource.Id).ToListAsync();
            _dbContext.ApiResourceClaims.RemoveRange(apiResourceClaims);
        }

        public async Task<int> UpdateApiResourceAsync(ApiResource apiResource)
        {
            //Remove old relations
            await RemoveApiResourceClaimsAsync(apiResource);

            //Update with new data
            _dbContext.ApiResources.Update(apiResource);

            return await AutoSaveChangesAsync();
        }

        public async Task<int> DeleteApiResourceAsync(ApiResource apiResource)
        {
            var resource = await _dbContext.ApiResources.Where(x => x.Id == apiResource.Id).SingleOrDefaultAsync();

            _dbContext.Remove(resource);

            return await AutoSaveChangesAsync();
        }

        public async Task<PagedList<ApiScope>> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiScope>();

            var apiScopes = await _dbContext.ApiScopes
                .Include(x => x.ApiResource)
                .Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Name, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(apiScopes);
            pagedList.TotalCount = await _dbContext.ApiScopes.Where(x => x.ApiResource.Id == apiResourceId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<ApiScope> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            return _dbContext.ApiScopes
                .Include(x => x.UserClaims)
                .Include(x => x.ApiResource)
                .Where(x => x.Id == apiScopeId && x.ApiResource.Id == apiResourceId)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Add new api scope
        /// </summary>
        /// <param name="apiResourceId"></param>
        /// <param name="apiScope"></param>
        /// <returns>This method return new api scope id</returns>
        public async Task<int> AddApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            var apiResource = await _dbContext.ApiResources.Where(x => x.Id == apiResourceId).SingleOrDefaultAsync();
            apiScope.ApiResource = apiResource;

            _dbContext.ApiScopes.Add(apiScope);

            await AutoSaveChangesAsync();

            return apiScope.Id;
        }

        private async Task RemoveApiScopeClaimsAsync(ApiScope apiScope)
        {
            //Remove old api scope claims
            var apiScopeClaims = await _dbContext.ApiScopeClaims.Where(x => x.ApiScope.Id == apiScope.Id).ToListAsync();
            _dbContext.ApiScopeClaims.RemoveRange(apiScopeClaims);
        }

        public async Task<int> UpdateApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            var apiResource = await _dbContext.ApiResources.Where(x => x.Id == apiResourceId).SingleOrDefaultAsync();
            apiScope.ApiResource = apiResource;

            //Remove old relations
            await RemoveApiScopeClaimsAsync(apiScope);

            //Update with new data
            _dbContext.ApiScopes.Update(apiScope);

            return await AutoSaveChangesAsync();
        }

        public async Task<int> DeleteApiScopeAsync(ApiScope apiScope)
        {
            var apiScopeToDelete = await _dbContext.ApiScopes.Where(x => x.Id == apiScope.Id).SingleOrDefaultAsync();
            _dbContext.ApiScopes.Remove(apiScopeToDelete);

            return await AutoSaveChangesAsync();
        }

        public async Task<PagedList<ApiSecret>> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiSecret>();
            var apiSecrets = await _dbContext.ApiSecrets.Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(apiSecrets);
            pagedList.TotalCount = await _dbContext.ApiSecrets.Where(x => x.ApiResource.Id == apiResourceId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<ApiSecret> GetApiSecretAsync(int apiSecretId)
        {
            return _dbContext.ApiSecrets
                .Include(x => x.ApiResource)
                .Where(x => x.Id == apiSecretId)
                .SingleOrDefaultAsync();
        }

        public async Task<int> AddApiSecretAsync(int apiResourceId, ApiSecret apiSecret)
        {
            apiSecret.ApiResource = await _dbContext.ApiResources.Where(x => x.Id == apiResourceId).SingleOrDefaultAsync();
            await _dbContext.ApiSecrets.AddAsync(apiSecret);

            return await AutoSaveChangesAsync();
        }

        public async Task<int> DeleteApiSecretAsync(ApiSecret apiSecret)
        {
            var apiSecretToDelete = await _dbContext.ApiSecrets.Where(x => x.Id == apiSecret.Id).SingleOrDefaultAsync();
            _dbContext.ApiSecrets.Remove(apiSecretToDelete);

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

        public async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            var apiResourceName = await _dbContext.ApiResources.Where(x => x.Id == apiResourceId).Select(x => x.Name).SingleOrDefaultAsync();

            return apiResourceName;
        }
    }
}