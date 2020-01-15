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
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Skoruba.MultiTenant.Stores
{
    public abstract class EFCoreStoreDbContextBase<TTentantInfo> : DbContext
        where TTentantInfo : class, ITenantEntity
    {
        public DbSet<TTentantInfo> TenantInfo { get; set; }

        public EFCoreStoreDbContextBase(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TTentantInfo>().HasKey(ti => ti.Id);
            modelBuilder.Entity<TTentantInfo>().Property(ti => ti.Id).HasMaxLength(Finbuckle.MultiTenant.Core.Constants.TenantIdMaxLength);
            modelBuilder.Entity<TTentantInfo>().HasIndex(ti => ti.Identifier).IsUnique();
            modelBuilder.Entity<TTentantInfo>().Property(ti => ti.Name).IsRequired();
            modelBuilder.Entity<TTentantInfo>().Property(ti => ti.ConnectionString).IsRequired();
            modelBuilder.Entity<TTentantInfo>().ToTable("Tenants");
            modelBuilder.Entity<TTentantInfo>()
                .Property(b => b.Items)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v));
        }
    }
}