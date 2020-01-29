using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Identity.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class AdminIdentityDbContext : IdentityDbContext<UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>
    {
        private readonly bool _isMultiTenant = false;

        public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options) : base(options)
        {
        }

        public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options, bool isMultiTenant) : base(options)
        {
            _isMultiTenant = isMultiTenant;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureIdentityContext(builder);

            ConfigureIdentityForMultiTenant(builder);
        }
        protected void ConfigureIdentityForMultiTenant(ModelBuilder builder)
        {
            var configuration = Database.TryGetService<MultiTenantConfiguration>();
            
            if (configuration?.MultiTenantEnabled ?? _isMultiTenant)
            {
                var userBuilder = builder.Entity<UserIdentity>();
                userBuilder.RemoveIndex("NormalizedUserName");
                userBuilder.HasIndex("NormalizedUserName", "TenantId").HasName("UserNameIndex").IsUnique();

                var roleBuilder = builder.Entity<UserIdentityRole>();
                roleBuilder.RemoveIndex("NormalizedName");
                roleBuilder.HasIndex("NormalizedName", "TenantId").HasName("RoleNameIndex").IsUnique();
            }
        }
        private void ConfigureIdentityContext(ModelBuilder builder)
        {
            builder.Entity<UserIdentityRole>().ToTable(TableConsts.IdentityRoles);
            builder.Entity<UserIdentityRoleClaim>().ToTable(TableConsts.IdentityRoleClaims);
            builder.Entity<UserIdentityUserRole>().ToTable(TableConsts.IdentityUserRoles);

            builder.Entity<UserIdentity>().ToTable(TableConsts.IdentityUsers);
            builder.Entity<UserIdentityUserLogin>().ToTable(TableConsts.IdentityUserLogins);
            builder.Entity<UserIdentityUserClaim>().ToTable(TableConsts.IdentityUserClaims);
            builder.Entity<UserIdentityUserToken>().ToTable(TableConsts.IdentityUserTokens);
        }
    }
}