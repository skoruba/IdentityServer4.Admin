using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using Skoruba.IdentityServer4.Audit.Core.Entities;

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
                audit.ToTable("Audits");
                audit.HasKey(x => x.Id);
            });
        }
    }
}