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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skoruba.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Finbuckle.MultiTenant;

namespace Skoruba.MultiTenant
{
    /// <summary>
    /// An Identity database context that enforces tenant integrity on entity types
    /// marked with the MultiTenant annotation or attribute.
    /// <remarks>
    /// All Identity entity types are multitenant by default.
    /// </remarks>
    /// </summary>
    public abstract class MultiTenantIdentityDbContext : MultiTenantIdentityDbContext<IdentityUser>
    {
        protected MultiTenantIdentityDbContext(TenantInfo tenantInfo) : base(tenantInfo)
        {
        }

        protected MultiTenantIdentityDbContext(TenantInfo tenantInfo, DbContextOptions options) : base(tenantInfo, options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>().IsMultiTenant();
        }
    }

    /// <summary>
    /// An Identity database context that enforces tenant integrity on entity types
    /// marked with the MultiTenant annotation or attribute.
    /// <remarks>
    /// TUser is not multitenant by default.
    /// All other Identity entity types are multitenant by default.
    /// </remarks>
    /// </summary>
    public abstract class MultiTenantIdentityDbContext<TUser> : MultiTenantIdentityDbContext<TUser, IdentityRole, string>
        where TUser : IdentityUser
    {
        protected MultiTenantIdentityDbContext(TenantInfo tenantInfo) : base(tenantInfo)
        {
        }

        protected MultiTenantIdentityDbContext(TenantInfo tenantInfo, DbContextOptions options) : base(tenantInfo, options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().IsMultiTenant();
        }
    }

    /// <summary>
    /// An Identity database context that enforces tenant integrity on entity types
    /// marked with the MultiTenant annotation or attribute.
    /// <remarks>
    /// TUser and TRole are not multitenant by default.
    /// All other Identity entity types are multitenant by default.
    /// </remarks>
    /// </summary>
    public abstract class MultiTenantIdentityDbContext<TUser, TRole, TKey> : MultiTenantIdentityDbContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        protected MultiTenantIdentityDbContext(TenantInfo tenantInfo) : base(tenantInfo)
        {
        }

        protected MultiTenantIdentityDbContext(TenantInfo tenantInfo, DbContextOptions options) : base(tenantInfo, options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserClaim<TKey>>().IsMultiTenant();
            builder.Entity<IdentityUserRole<TKey>>().IsMultiTenant();
            builder.Entity<IdentityUserLogin<TKey>>().IsMultiTenant();
            builder.Entity<IdentityRoleClaim<TKey>>().IsMultiTenant();
            builder.Entity<IdentityUserToken<TKey>>().IsMultiTenant();
        }
    }

    /// <summary>
    /// An Identity database context that enforces tenant integrity on entity types
    /// marked with the MultiTenant annotation or attribute.
    /// <remarks>
    /// No Identity entity types are multitenant by default.
    /// </remarks>
    /// </summary>
    public abstract class MultiTenantIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IMultiTenantDbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TKey : IEquatable<TKey>
    {
        public TenantInfo TenantInfo { get; }

        public TenantMismatchMode TenantMismatchMode { get; set; } = TenantMismatchMode.Throw;

        public TenantNotSetMode TenantNotSetMode { get; set; } = TenantNotSetMode.Throw;

        [Obsolete]
        internal IImmutableList<IEntityType> MultiTenantEntityTypes
        {
            get
            {
                return Model.GetMultiTenantEntityTypes().ToImmutableList();
            }
        }

        protected string ConnectionString => TenantInfo.ConnectionString;

        protected MultiTenantIdentityDbContext(TenantInfo tenantInfo)
        {
            this.TenantInfo = tenantInfo;
        }

        protected MultiTenantIdentityDbContext(TenantInfo tenantInfo, DbContextOptions options) : base(options)
        {
            this.TenantInfo = tenantInfo;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigureMultiTenant();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.EnforceMultiTenant();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
                                                         CancellationToken cancellationToken = default(CancellationToken))
        {
            this.EnforceMultiTenant();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

}