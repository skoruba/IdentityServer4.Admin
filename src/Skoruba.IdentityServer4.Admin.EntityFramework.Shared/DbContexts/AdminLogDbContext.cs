using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class AdminLogDbContext : DbContext, IAdminLogDbContext
    {
        protected virtual Guid? CurrentTenantId => CurrentTenant?.Id;
        public ICurrentTenant CurrentTenant { get; set; }
        public DbSet<Log> Logs { get; set; }

        public AdminLogDbContext(DbContextOptions<AdminLogDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureLogContext(builder);
        }

        private void ConfigureLogContext(ModelBuilder builder)
        {
            builder.Entity<Log>(log =>
            {
                log.ToTable(TableConsts.Logging);
                log.HasKey(x => x.Id);
                log.Property(x => x.Level).HasMaxLength(128);
                log.HasQueryFilter(r => r.TenantId == CurrentTenantId);
            });
        }
    }
}
