using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Enums;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Extensions;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories
{
    public class ApiResourceRepository<TDbContext> : IApiResourceRepository
        where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        protected readonly TDbContext DbContext;

        public bool AutoSaveChanges { get; set; } = true;

        public ApiResourceRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<PagedList<ApiResource>> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiResource>();
            Expression<Func<ApiResource, bool>> searchCondition = x => x.Name.Contains(search);

            var apiResources = await DbContext.ApiResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Name, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(apiResources);
            pagedList.TotalCount = await DbContext.ApiResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ApiResource> GetApiResourceAsync(int apiResourceId)
        {
            return DbContext.ApiResources
                .Include(x => x.UserClaims)
                .Where(x => x.Id == apiResourceId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public virtual async Task<PagedList<ApiResourceProperty>> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiResourceProperty>();

            var properties = await DbContext.ApiResourceProperties.Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(properties);
            pagedList.TotalCount = await DbContext.ApiResourceProperties.Where(x => x.ApiResource.Id == apiResourceId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ApiResourceProperty> GetApiResourcePropertyAsync(int apiResourcePropertyId)
        {
            return DbContext.ApiResourceProperties
                .Include(x => x.ApiResource)
                .Where(x => x.Id == apiResourcePropertyId)
                .SingleOrDefaultAsync();
        }

        public virtual async Task<int> AddApiResourcePropertyAsync(int apiResourceId, ApiResourceProperty apiResourceProperty)
        {
            var apiResource = await DbContext.ApiResources.Where(x => x.Id == apiResourceId).SingleOrDefaultAsync();

            apiResourceProperty.ApiResource = apiResource;
            await DbContext.ApiResourceProperties.AddAsync(apiResourceProperty);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
        {
            var propertyToDelete = await DbContext.ApiResourceProperties.Where(x => x.Id == apiResourceProperty.Id).SingleOrDefaultAsync();

            DbContext.ApiResourceProperties.Remove(propertyToDelete);
            return await AutoSaveChangesAsync();
        }

        public virtual async Task<bool> CanInsertApiResourceAsync(ApiResource apiResource)
        {
            if (apiResource.Id == 0)
            {
                var existsWithSameName = await DbContext.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await DbContext.ApiResources.Where(x => x.Name == apiResource.Name && x.Id != apiResource.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
        {
            var existsWithSameName = await DbContext.ApiResourceProperties.Where(x => x.Key == apiResourceProperty.Key
                                                                                       && x.ApiResource.Id == apiResourceProperty.ApiResourceId).SingleOrDefaultAsync();
            return existsWithSameName == null;
        }

        public virtual async Task<bool> CanInsertApiScopeAsync(ApiScope apiScope)
        {
            if (apiScope.Id == 0)
            {
                var existsWithSameName = await DbContext.ApiScopes.Where(x => x.Name == apiScope.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await DbContext.ApiScopes.Where(x => x.Name == apiScope.Name && x.Id != apiScope.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        /// <summary>
        /// Add new api resource
        /// </summary>
        /// <param name="apiResource"></param>
        /// <returns>This method return new api resource id</returns>
        public virtual async Task<int> AddApiResourceAsync(ApiResource apiResource)
        {
            DbContext.ApiResources.Add(apiResource);

            await AutoSaveChangesAsync();

            return apiResource.Id;
        }

        private async Task RemoveApiResourceClaimsAsync(ApiResource identityResource)
        {
            //Remove old identity claims
            var apiResourceClaims = await DbContext.ApiResourceClaims.Where(x => x.ApiResource.Id == identityResource.Id).ToListAsync();
            DbContext.ApiResourceClaims.RemoveRange(apiResourceClaims);
        }

        public virtual async Task<int> UpdateApiResourceAsync(ApiResource apiResource)
        {
            //Remove old relations
            await RemoveApiResourceClaimsAsync(apiResource);

            //Update with new data
            DbContext.ApiResources.Update(apiResource);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteApiResourceAsync(ApiResource apiResource)
        {
            var resource = await DbContext.ApiResources.Where(x => x.Id == apiResource.Id).SingleOrDefaultAsync();

            DbContext.Remove(resource);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<PagedList<ApiScope>> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiScope>();

            var apiScopes = await DbContext.ApiScopes
                .Include(x => x.ApiResource)
                .Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Name, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(apiScopes);
            pagedList.TotalCount = await DbContext.ApiScopes.Where(x => x.ApiResource.Id == apiResourceId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ApiScope> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            return DbContext.ApiScopes
                .Include(x => x.UserClaims)
                .Include(x => x.ApiResource)
                .Where(x => x.Id == apiScopeId && x.ApiResource.Id == apiResourceId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Add new api scope
        /// </summary>
        /// <param name="apiResourceId"></param>
        /// <param name="apiScope"></param>
        /// <returns>This method return new api scope id</returns>
        public virtual async Task<int> AddApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            var apiResource = await DbContext.ApiResources.Where(x => x.Id == apiResourceId).SingleOrDefaultAsync();
            apiScope.ApiResource = apiResource;

            DbContext.ApiScopes.Add(apiScope);

            await AutoSaveChangesAsync();

            return apiScope.Id;
        }

        private async Task RemoveApiScopeClaimsAsync(ApiScope apiScope)
        {
            //Remove old api scope claims
            var apiScopeClaims = await DbContext.ApiScopeClaims.Where(x => x.ApiScope.Id == apiScope.Id).ToListAsync();
            DbContext.ApiScopeClaims.RemoveRange(apiScopeClaims);
        }

        public virtual async Task<int> UpdateApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            var apiResource = await DbContext.ApiResources.Where(x => x.Id == apiResourceId).SingleOrDefaultAsync();
            apiScope.ApiResource = apiResource;

            //Remove old relations
            await RemoveApiScopeClaimsAsync(apiScope);

            //Update with new data
            DbContext.ApiScopes.Update(apiScope);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteApiScopeAsync(ApiScope apiScope)
        {
            var apiScopeToDelete = await DbContext.ApiScopes.Where(x => x.Id == apiScope.Id).SingleOrDefaultAsync();
            DbContext.ApiScopes.Remove(apiScopeToDelete);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<PagedList<ApiSecret>> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiSecret>();
            var apiSecrets = await DbContext.ApiSecrets.Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(apiSecrets);
            pagedList.TotalCount = await DbContext.ApiSecrets.Where(x => x.ApiResource.Id == apiResourceId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ApiSecret> GetApiSecretAsync(int apiSecretId)
        {
            return DbContext.ApiSecrets
                .Include(x => x.ApiResource)
                .Where(x => x.Id == apiSecretId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public virtual async Task<int> AddApiSecretAsync(int apiResourceId, ApiSecret apiSecret)
        {
            apiSecret.ApiResource = await DbContext.ApiResources.Where(x => x.Id == apiResourceId).SingleOrDefaultAsync();
            await DbContext.ApiSecrets.AddAsync(apiSecret);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteApiSecretAsync(ApiSecret apiSecret)
        {
            var apiSecretToDelete = await DbContext.ApiSecrets.Where(x => x.Id == apiSecret.Id).SingleOrDefaultAsync();
            DbContext.ApiSecrets.Remove(apiSecretToDelete);

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

        public virtual async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            var apiResourceName = await DbContext.ApiResources.Where(x => x.Id == apiResourceId).Select(x => x.Name).SingleOrDefaultAsync();

            return apiResourceName;
        }
    }
}