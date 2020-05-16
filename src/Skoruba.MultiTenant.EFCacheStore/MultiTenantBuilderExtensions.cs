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

using Microsoft.EntityFrameworkCore;
using Skoruba.MultiTenant.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provices builder methods for Skoruba.Multitenancy services and configuration.
    /// </summary>
    public static class FinbuckleMultiTenantBuilderExtensions
    {
        /// <summary>
        /// Adds the default EFCacheStore and dbcontext.  The store will cache results for 48 hours.
        /// </summary>
        /// <returns>The same MultiTenantBuilder passed into the method.</returns>
        public static FinbuckleMultiTenantBuilder WithEFCacheStore(this FinbuckleMultiTenantBuilder builder, Action<DbContextOptionsBuilder> options)
        {
            builder.Services.AddDbContext<EFCoreStoreDbContext>(options); // Note, will not override existing context if already added.
            return builder.WithStore<EFCoreCacheStore<EFCoreStoreDbContext, TenantEntity>>(ServiceLifetime.Scoped);
        }
    }
}