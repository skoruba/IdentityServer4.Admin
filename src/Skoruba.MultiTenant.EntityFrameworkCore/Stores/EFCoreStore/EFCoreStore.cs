//    Copyright 2018 Andrew White
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;

namespace Skoruba.Multitenancy.Stores
{
    public class EFCoreStore<TEFCoreStoreDbContext> : IMultiTenantStore
        where TEFCoreStoreDbContext : EFCoreStoreDbContext
    {
        private readonly TEFCoreStoreDbContext dbContext;

        public EFCoreStore(TEFCoreStoreDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<TenantInfo> TryGetAsync(string id)
        {
            return await dbContext.Set<TenantInfo>()
                            .Where(ti => ti.Id == id)
                            .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, null))
                            .SingleOrDefaultAsync();
        }

        public async Task<TenantInfo> TryGetByIdentifierAsync(string identifier)
        {
            return await dbContext.Set<TenantInfo>()
                            .Where(ti => ti.Identifier == identifier)
                            .Select(ti => new TenantInfo(ti.Id, ti.Identifier, ti.Name, ti.ConnectionString, null))
                            .SingleOrDefaultAsync();
        }

        public async Task<bool> TryAddAsync(TenantInfo tenantInfo)
        {
            var newEntity = new TenantInfo
            {
                Id = tenantInfo.Id,
                Identifier = tenantInfo.Identifier,
                Name = tenantInfo.Name,
                ConnectionString = tenantInfo.ConnectionString
            };

            await dbContext.Set<TenantInfo>().AddAsync(newEntity);

            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> TryRemoveAsync(string identifier)
        {
            var existing = await dbContext.Set<TenantInfo>().Where(ti => ti.Identifier == identifier).FirstOrDefaultAsync();
            dbContext.Set<TenantInfo>().Remove(existing);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> TryUpdateAsync(TenantInfo tenantInfo)
        {
            var existing = await dbContext.Set<TenantInfo>().FindAsync(tenantInfo.Id);
            existing.Identifier = tenantInfo.Identifier;
            existing.Name = tenantInfo.Name;
            existing.ConnectionString = tenantInfo.ConnectionString;

            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}