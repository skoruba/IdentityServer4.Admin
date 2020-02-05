using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Skoruba.MultiTenant.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Stores
{
    public class EFCoreCacheStore<TEFCoreStoreDbContext, TTenantInfo> : IMultiTenantStore
        where TEFCoreStoreDbContext : DbContext
        where TTenantInfo : class, ITenantEntity, new()
    {
        private readonly TEFCoreStoreDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private const string _cacheKey = "Skoruba.MultiTenant.EfCoreCacheStore";

        private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(48));

        public EFCoreCacheStore(TEFCoreStoreDbContext dbContext, IMemoryCache memoryCache, MultiTenantConfiguration multiTenantConfiguration)
        {
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(multiTenantConfiguration.TenantStoreCacheHours));
        }

        private async Task<List<TenantInfo>> GetTenantsFromCacheAsync()
        {
            if (!_memoryCache.TryGetValue(_cacheKey, out List<TenantInfo> cachedTenants))
            {
                // key is not in cache, so get data
                cachedTenants = await _dbContext.Set<TTenantInfo>()
                    .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, ti.Items))
                    .ToListAsync();

                // Save data in cache
                _memoryCache.Set(_cacheKey, cachedTenants, _cacheOptions);
            }

            return cachedTenants;
        }

        public async Task<TenantInfo> TryGetAsync(string id)
        {
            var cachedTenants = await GetTenantsFromCacheAsync();
            var result = cachedTenants.SingleOrDefault(ti => ti.Id == id);

            if (result == null)
            {
                var results = await _dbContext.Set<TTenantInfo>()
                            .Where(ti => ti.Id == id)
                            .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, ti.Items))
                            .SingleOrDefaultAsync();

                if (result != null)
                {
                    _memoryCache.Remove(_cacheKey);
                }
            }

            return result;
        }

        public async Task<TenantInfo> TryGetByIdentifierAsync(string identifier)
        {
            var cachedTenants = await GetTenantsFromCacheAsync();
            var result = cachedTenants.SingleOrDefault(ti => ti.Identifier == identifier);
            if (result == null)
            {
                var results = await _dbContext.Set<TTenantInfo>()
                            .Where(ti => ti.Identifier == identifier)
                            .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, ti.Items))
                            .SingleOrDefaultAsync();

                if (result != null)
                {
                    _memoryCache.Remove(_cacheKey);
                }
            }

            return result;
        }

        public async Task<bool> TryAddAsync(TenantInfo tenantInfo)
        {
            var newEntity = new TTenantInfo
            {
                Id = tenantInfo.Id,
                Identifier = tenantInfo.Identifier,
                Name = tenantInfo.Name,
                ConnectionString = tenantInfo.ConnectionString,
                Items = tenantInfo.Items
            };

            await _dbContext.Set<TTenantInfo>().AddAsync(newEntity);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> TryRemoveAsync(string identifier)
        {
            var existing = await _dbContext.Set<TTenantInfo>()
                .Where(ti => ti.Identifier == identifier)
                .FirstOrDefaultAsync();
            _dbContext.Set<TTenantInfo>().Remove(existing);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> TryUpdateAsync(TenantInfo tenantInfo)
        {
            var existing = await _dbContext.Set<TTenantInfo>().FindAsync(tenantInfo.Id);
            existing.Identifier = tenantInfo.Identifier;
            existing.Name = tenantInfo.Name;
            existing.ConnectionString = tenantInfo.ConnectionString;
            existing.Items = tenantInfo.Items;

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }

}