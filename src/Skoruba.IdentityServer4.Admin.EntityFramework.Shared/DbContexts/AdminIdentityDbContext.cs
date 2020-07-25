using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entitites.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Constants;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class AdminIdentityDbContext : IdentityDbContext<UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>
    {
        protected virtual Guid? CurrentTenantId => CurrentTenant?.Id;
        public ICurrentTenant CurrentTenant { get; set; }

        public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureIdentityContext(builder);
        }

        private void ConfigureIdentityContext(ModelBuilder builder)
        {
            builder.Entity<UserIdentityRole>().ToTable(TableConsts.IdentityRoles)
                .HasQueryFilter(r => r.TenantId == CurrentTenantId)
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<UserIdentityRoleClaim>().ToTable(TableConsts.IdentityRoleClaims);
            builder.Entity<UserIdentityUserRole>().ToTable(TableConsts.IdentityUserRoles);

            builder.Entity<UserIdentity>().ToTable(TableConsts.IdentityUsers)
                .HasQueryFilter(r => r.TenantId == CurrentTenantId)
                .Property(e => e.Id)
                .ValueGeneratedOnAdd(); ;
            builder.Entity<UserIdentityUserLogin>().ToTable(TableConsts.IdentityUserLogins);
            builder.Entity<UserIdentityUserClaim>().ToTable(TableConsts.IdentityUserClaims);
            builder.Entity<UserIdentityUserToken>().ToTable(TableConsts.IdentityUserTokens);
        }
    }
}