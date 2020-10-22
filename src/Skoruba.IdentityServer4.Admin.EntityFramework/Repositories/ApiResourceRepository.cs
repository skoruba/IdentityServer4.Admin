using System;
using System.Collections.Generic;
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

			var properties = await DbContext.ApiResourceProperties
											.Where(x => x.ApiResource.Id == apiResourceId)
											.PageBy(x => x.Id, page, pageSize)
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
							.SingleOrDefaultAsync(x => x.Id == apiResourcePropertyId);
        }

        public virtual async Task<int> AddApiResourcePropertyAsync(int apiResourceId, ApiResourceProperty apiResourceProperty)
        {
            var apiResource = await DbContext.ApiResources.SingleOrDefaultAsync(x => x.Id == apiResourceId);

            apiResourceProperty.ApiResource = apiResource;
            await DbContext.ApiResourceProperties.AddAsync(apiResourceProperty);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
        {
            var propertyToDelete = await DbContext.ApiResourceProperties.SingleOrDefaultAsync(x => x.Id == apiResourceProperty.Id);

            DbContext.ApiResourceProperties.Remove(propertyToDelete);
            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteApiResourceScopeAsync(ApiResourceScope apiResourceScope)
        {
            var resourceScopeToDelete = await DbContext.ApiResourceScopes.SingleOrDefaultAsync(x => x.Id == apiResourceScope.Id);

            DbContext.ApiResourceScopes.Remove(resourceScopeToDelete);
            return await AutoSaveChangesAsync();
        }

        public virtual async Task<bool> CanInsertApiResourceAsync(ApiResource apiResource)
        {
            if (apiResource.Id == 0)
            {
                var existsWithSameName = await DbContext.ApiResources.SingleOrDefaultAsync(x => x.Name == apiResource.Name);
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await DbContext.ApiResources.SingleOrDefaultAsync(x => x.Name == apiResource.Name && x.Id != apiResource.Id);
                return existsWithSameName == null;
            }
        }

        public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
        {
            var existsWithSameName = await DbContext.ApiResourceProperties
                                                    .SingleOrDefaultAsync(x => x.Key == apiResourceProperty.Key && x.ApiResource.Id == apiResourceProperty.ApiResourceId);
            return existsWithSameName == null;
        }
        public virtual async Task<bool> CanInsertApiResourceScopeAsync(ApiResourceScope apiResourceScope)
        {
            var existsWithSameName = await DbContext.ApiResourceScopes
                                                    .SingleOrDefaultAsync(x => x.Scope.Equals(apiResourceScope.Scope) && x.ApiResource.Id == apiResourceScope.ApiResourceId);
            return existsWithSameName == null;
        }

        public virtual async Task<bool> CanInsertApiResourceSecretAsync(ApiScope apiScope)
        {
            if (apiScope.Id == 0)
            {
                var existsWithSameName = await DbContext.ApiScopes
                                                        .SingleOrDefaultAsync(x => x.Name == apiScope.Name);
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await DbContext.ApiScopes
                                                        .SingleOrDefaultAsync(x => x.Name == apiScope.Name && x.Id != apiScope.Id);
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

		public async Task<PagedList<ApiScope>> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
		{
			// Se filtran los Scopes asociados al ApiResource, registrados en la tabla intermedia
			var apiResourceScopes = await DbContext.ApiResourceScopes
												   .Where(x => x.ApiResourceId == apiResourceId)
												   .PageBy(x => x.Id, page, pageSize)
												   .Select(x => x.Scope)
												   .ToListAsync();

            // Se obtienen todos los Scopes que estén registrados en la tabla intermedia
			var apiScopes = await DbContext.ApiScopes
                                           .Include(x => x.UserClaims)
                                           .Include(x => x.Properties)
                                           .Where(x => apiResourceScopes.Contains(x.Name))
										   .PageBy(x => x.Id, page, pageSize)
										   .ToListAsync();

			var pagedList = new PagedList<ApiScope>();
			pagedList.Data.AddRange(apiScopes);
			pagedList.TotalCount = await DbContext.ApiResourceScopes.CountAsync(x => x.ApiResource.Id == apiResourceId);
			pagedList.PageSize = pageSize;

			return pagedList;
		}

		public virtual Task<ApiScope> GetApiScopeAsync(int apiScopeId)
		{
            return DbContext.ApiScopes
							.Include(x => x.UserClaims)
                            .Include(x => x.Properties)
                            .Where(x => x.Id == apiScopeId)
							.AsNoTracking()
							.SingleOrDefaultAsync();
        }

		public virtual Task<ApiScope> GetApiScopeAsync(string name)
        {
            return DbContext.ApiScopes
							.Include(x => x.UserClaims)
							.Include(x => x.Properties)
							.Where(x => x.Name == name)
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
			DbContext.ApiScopes.Add(apiScope);

			var apiResourceScope = new ApiResourceScope()
			{
                Id = apiScope.Id,
				Scope = apiScope.Name,
				ApiResourceId = apiResourceId
			};

			DbContext.ApiResourceScopes.Add(apiResourceScope);

			await AutoSaveChangesAsync();

            return apiScope.Id;
        }

        public virtual async Task<int> AddApiResourceScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            DbContext.ApiScopes.Add(apiScope);

            var apiResourceScope = new ApiResourceScope()
            {
                Id = apiScope.Id,
                Scope = apiScope.Name,
                ApiResourceId = apiResourceId
            };

            DbContext.ApiResourceScopes.Add(apiResourceScope);

            await AutoSaveChangesAsync();

            return apiScope.Id;
        }

        private async Task RemoveApiScopeClaimsAsync(ApiScope apiScope)
        {
            //Remove old api scope claims
            var apiScopeClaims = await DbContext.ApiScopeClaims.Where(x => x.ScopeId == apiScope.Id).ToListAsync();
            DbContext.ApiScopeClaims.RemoveRange(apiScopeClaims);
        }

        public virtual async Task<int> UpdateApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            ////Remove old relations
            await RemoveApiScopeClaimsAsync(apiScope);

            ///Remove properties

            //Update with new data
            DbContext.ApiScopes.Update(apiScope);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteApiScopeAsync(ApiScope apiScope)
        {
            var resourceScopeToDelete = await DbContext.ApiResourceScopes.SingleOrDefaultAsync(x => x.Scope.Equals(apiScope.Name));
            DbContext.ApiResourceScopes.Remove(resourceScopeToDelete);

            var apiScopeToDelete = await DbContext.ApiScopes.SingleOrDefaultAsync(x => x.Id == apiScope.Id);
            DbContext.ApiScopes.Remove(apiScopeToDelete);

			return await AutoSaveChangesAsync();
        }

		public virtual async Task<PagedList<ApiResourceSecret>> GetApiResourceSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ApiResourceSecret>();
            var ApiResourceSecret = await DbContext.ApiResourceSecrets.Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(ApiResourceSecret);
            pagedList.TotalCount = await DbContext.ApiResourceSecrets.CountAsync(x => x.ApiResource.Id == apiResourceId);
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ApiResourceSecret> GetApiResourceSecretAsync(int apiSecretId)
        {
            return DbContext.ApiResourceSecrets
							.Include(x => x.ApiResource)
                            .Where(x => x.Id == apiSecretId)
							.AsNoTracking()
							.SingleOrDefaultAsync();
        }

		public virtual async Task<int> AddApiResourceSecretAsync(int apiResourceId, ApiResourceSecret apiResourceSecret)
        {
            apiResourceSecret.ApiResource = await DbContext.ApiResources.SingleOrDefaultAsync(x => x.Id == apiResourceId);

            await DbContext.ApiResourceSecrets.AddAsync(apiResourceSecret);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteApiResourceSecretAsync(ApiResourceSecret apiResourceSecret)
        {
            var ApiResourceSecretToDelete = await DbContext.ApiResourceSecrets.Where(x => x.Id == apiResourceSecret.Id).SingleOrDefaultAsync();
            DbContext.ApiResourceSecrets.Remove(ApiResourceSecretToDelete);

            return await AutoSaveChangesAsync();
        }

        protected virtual async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public virtual async Task<string> GetApiResourceSecretNameAsync(int apiResourceId)
        {
            var apiResourceName = await DbContext.ApiResources.Where(x => x.Id == apiResourceId).Select(x => x.Name).SingleOrDefaultAsync();

            return apiResourceName;
        }
	}
}