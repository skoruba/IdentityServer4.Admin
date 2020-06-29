using System;
using System.Threading.Tasks;
using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class AdminAuditLogDbContext : DbContext, IAuditLoggingDbContext<AppAuditLog>
    {
        protected virtual Guid? CurrentTenantId => CurrentTenant?.Id;
        public ICurrentTenant CurrentTenant { get; set; }
        public AdminAuditLogDbContext(DbContextOptions<AdminAuditLogDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public DbSet<AppAuditLog> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppAuditLog>().ToTable(TableConsts.AuditLog).HasQueryFilter(r => r.TenantId == CurrentTenantId);
        }
    }
}
