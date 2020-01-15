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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Stores;

namespace Skoruba.MultiTenant.EntityFrameworkCore
{
    public static class DbMigrationTenantsHelpers
    {
        public static async Task EnsureSeed<TEFCoreStoreDbContext, TTenantInfo>(IHost host)
          where TEFCoreStoreDbContext : EFCoreStoreDbContextBase<TTenantInfo>
          where TTenantInfo : class, ITenantEntity, new()
        {
            using var serviceScope = host.Services.CreateScope();
            var services = serviceScope.ServiceProvider;
            await EnsureDatabasesMigrated<TEFCoreStoreDbContext>(services);
            await EnsureSeedData<TEFCoreStoreDbContext, TTenantInfo>(services);
        }

        public static async Task EnsureDatabasesMigrated<TEFCoreStoreDbContext>(IServiceProvider services)
            where TEFCoreStoreDbContext : DbContext
        {
            using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<TEFCoreStoreDbContext>();
            await context.Database.MigrateAsync();
        }

        public static async Task EnsureSeedData<TEFCoreStoreDbContext, TTenantInfo>(IServiceProvider services)
          where TEFCoreStoreDbContext : EFCoreStoreDbContextBase<TTenantInfo>
          where TTenantInfo : class, ITenantEntity, new()
        {

            using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var seedData = scope.ServiceProvider.GetRequiredService<MultiTenantSeedData>();

            using var context = scope.ServiceProvider.GetRequiredService<TEFCoreStoreDbContext>();
            foreach (var tenant in seedData.Tenants)
            {
                if (context.Set<TTenantInfo>().Find(tenant.Id) == null)
                {
                    var entity = new TTenantInfo()
                    {
                        Id = tenant.Id,
                        Identifier = tenant.Identifier,
                        ConnectionString = tenant.ConnectionString,
                        Name = tenant.Name,
                        Items = tenant.Items
                    };
                    context.Set<TTenantInfo>().Add(entity);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}