using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class MultiTenantDbContext : DbContext, IMultiTenantDbContext
    {
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Edition> Editions { get; set; }

        public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

        public MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureMultiTenant(modelBuilder);
        }

        private void ConfigureMultiTenant(ModelBuilder builder)
        {
            builder.Entity<Tenant>(b =>
            {
                b.ToTable(TableConsts.Tenant);
                b.Property(t => t.Name).IsRequired().HasMaxLength(TenantConsts.MaxNameLength);
                b.HasMany(u => u.ConnectionStrings).WithOne().HasForeignKey(uc => uc.TenantId).IsRequired();
                b.HasIndex(u => u.Name);
            });

            builder.Entity<TenantConnectionString>(b =>
            {
                b.ToTable(TableConsts.TenantConnectionString);
                b.HasKey(x => new { x.TenantId, x.Name });
                b.Property(cs => cs.Name).IsRequired().HasMaxLength(TenantConnectionStringConsts.MaxNameLength);
                b.Property(cs => cs.Value).IsRequired().HasMaxLength(TenantConnectionStringConsts.MaxValueLength);
            });

            builder.Entity<Edition>(b =>
            {
                b.ToTable(TableConsts.Edition);
                b.Property(t => t.Name).IsRequired().HasMaxLength(TenantConsts.MaxNameLength);
                b.HasMany(u => u.Tenants).WithOne(uc => uc.Edition).IsRequired();
                b.HasIndex(u => u.Name);
            });
        }
    }
}
