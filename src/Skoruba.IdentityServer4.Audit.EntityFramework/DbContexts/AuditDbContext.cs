using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using Skoruba.IdentityServer4.Audit.Core.Entities;
using Skoruba.IdentityServer4.Audit.EntityFramework.Constants;
using System;

namespace Skoruba.IdentityServer4.Audit.EntityFramework
{
    public class AuditDbContext : DbContext
    {
        public DbSet<AuditEntity> Audits { get; set; }

        public AuditDbContext(DbContextOptions<AuditDbContext> options)
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
            builder.Entity<AuditEntity>(audit =>
            {
                audit.ToTable(ConfigurationConsts.AuditLogTableName);
                audit.HasKey(x => x.Id);
                audit
                    .Property(p => p.TimeStamp)
                    .HasConversion(c => c, c => DateTime.SpecifyKind(c, DateTimeKind.Utc));
            });
        }
    }
}