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
            builder.Entity<UserIdentityRole>().ToTable(TableConsts.IdentityRoles).HasQueryFilter(r => r.TenantId == CurrentTenantId);
            builder.Entity<UserIdentityRoleClaim>().ToTable(TableConsts.IdentityRoleClaims).HasQueryFilter(r => r.TenantId == CurrentTenantId);
            builder.Entity<UserIdentityUserRole>().ToTable(TableConsts.IdentityUserRoles).HasQueryFilter(r => r.TenantId == CurrentTenantId);

            builder.Entity<UserIdentity>().ToTable(TableConsts.IdentityUsers).HasQueryFilter(r => r.TenantId == CurrentTenantId);
            builder.Entity<UserIdentityUserLogin>().ToTable(TableConsts.IdentityUserLogins).HasQueryFilter(r => r.TenantId == CurrentTenantId);
            builder.Entity<UserIdentityUserClaim>().ToTable(TableConsts.IdentityUserClaims).HasQueryFilter(r => r.TenantId == CurrentTenantId);
            builder.Entity<UserIdentityUserToken>().ToTable(TableConsts.IdentityUserTokens).HasQueryFilter(r => r.TenantId == CurrentTenantId);
        }
    }
}