using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores
{
    public class TenantStore : ITenantStore
    {
        private bool _disposed;
        private IMemoryCache _cache;
        private string _cacheKey = "tenants";

        private MemoryCacheEntryOptions cacheOptions => new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(48));

        private async Task<List<Tenant>> GetTenantsFromCacheAsync()
        {
            List<Tenant> cachedTenants;
            if (!_cache.TryGetValue(_cacheKey, out cachedTenants))
            {
                // key is not in cache, so get data
                cachedTenants = await Tenants.ToListAsync();

                // Save data in cache
                _cache.Set(_cacheKey, cachedTenants, cacheOptions);
            }

            return cachedTenants;
        }

        private async Task<Tenant> GetTenantFromCacheAsync(string id)
        {
            Tenant cachedTenant;
            List<Tenant> cachedTenants;
            if (!_cache.TryGetValue(_cacheKey, out cachedTenants))
            {
                // key is not in cache, so get data
                cachedTenants = await Tenants.ToListAsync();

                // Save data in cache
                _cache.Set(_cacheKey, cachedTenants, cacheOptions);

                cachedTenant = cachedTenants.FirstOrDefault(a => a.Id == id);
            }
            else
            {
                // Get the tenant from the cache
                cachedTenant = cachedTenants.FirstOrDefault(a => a.Id == id);

                if (cachedTenant == null)
                {
                    // If tenant was not found then get the tenant from the store
                    var storeTenant = await FindByIdAsync(id, default);

                    if (storeTenant != null)
                    {
                        // If tenant is not null then add it to the cache
                        cachedTenants.Add(storeTenant);

                        // Reset the cache
                        ClearCache();

                        _cache.Set(_cacheKey, cachedTenant, cacheOptions);
                    }

                    cachedTenant = storeTenant;
                }
            }

            return cachedTenant;
        }

        private void ClearCache()
        {
            _cache.Remove(_cacheKey);
        }

        private MultiTenantUserIdentityDbContext Context { get; set; }

        public TenantStore(MultiTenantUserIdentityDbContext context, IMemoryCache cache)
        {
            _cache = cache;
            Context = context;
        }

        public async Task CreateAsync(Tenant tenant, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            Context.Tenants.Add(tenant);
            await Context.SaveChangesAsync(cancellationToken);
            ClearCache();
        }

        public async Task DeleteAsync(Tenant tenant, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            Context.Tenants.Remove(tenant);
            await Context.SaveChangesAsync(cancellationToken);
            ClearCache();
        }

        public async Task<Tenant> FindByIdAsync(string tenantId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return await Context.Tenants.FindAsync(tenantId);
            //return (await GetTenantsFromCacheAsync())
            //    .FirstOrDefault(a => a.Id == tenantId);
        }

        public async Task<Tenant> FindByIdFromCacheAsync(string tenantId)
        {
            return await GetTenantFromCacheAsync(tenantId);
        }

        public async Task<Tenant> FindByNameAsync(string normalizedtenantName, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return await Context.Tenants.FirstOrDefaultAsync(t => t.Name.ToUpper() == normalizedtenantName.ToUpper(), cancellationToken);
            //return (await GetTenantsFromCacheAsync())
            //    .FirstOrDefault(a => a.Name.ToUpper() == normalizedtenantName.ToUpper());
        }

        public async Task<Tenant> FindByCodeAsync(string tenantCode, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return await Context.Tenants.SingleOrDefaultAsync(t => t.Code.ToUpper() == tenantCode.ToUpper(), cancellationToken);
            //return (await GetTenantsFromCacheAsync())
            //    .SingleOrDefault(t => t.Code.ToUpper() == tenantCode.ToUpper());
        }

        public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            Context.Entry(tenant).State = EntityState.Modified;
            await Context.SaveChangesAsync(cancellationToken);
            ClearCache();
        }

        /// <summary>
        ///     If true will call dispose on the DbContext during Dipose
        /// </summary>
        public bool DisposeContext { get; set; }

        public IQueryable<Tenant> Tenants => Context.Tenants;

        /// <summary>
        ///     Dispose the store
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (DisposeContext && disposing && Context != null)
            {
                Context.Dispose();
            }
            _disposed = true;
            Context = null;
        }
    }
}