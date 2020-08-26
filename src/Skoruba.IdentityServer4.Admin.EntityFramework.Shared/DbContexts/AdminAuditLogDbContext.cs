using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Constants;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class AdminAuditLogDbContext : DbContext, IAuditLoggingDbContext<AuditLog>
    {
        public AdminAuditLogDbContext(DbContextOptions<AdminAuditLogDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureAdminAuditLogContext(builder);
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public DbSet<AuditLog> AuditLog { get; set; }

        private void ConfigureAdminAuditLogContext(ModelBuilder builder)
        {
            builder.Entity<AuditLog>(log =>
            {
                log.ToTable(TableConsts.AuditLogging);
                log.HasKey(x => x.Id);
                log.Property(x => x.Data).HasMaxLength(50000);
                log.Property(x => x.SubjectAdditionalData).HasMaxLength(50000);
            });
        }
    }
}
