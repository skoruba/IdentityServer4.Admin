using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class MultiTenantUserIdentityDbContext :
        IdentityDbContext<MultiTenantUserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>,
        IMultiTenantDbContext
    {
        public DbSet<Tenant> Tenants { get; set; }

        public MultiTenantUserIdentityDbContext(DbContextOptions<MultiTenantUserIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            IdentityUserContextBase(builder);

            IdentityDbContextBase(builder);

            ConfigureIdentityContext(builder);
        }

        private class PersonalDataConverter : ValueConverter<string, string>
        {
            public PersonalDataConverter(IPersonalDataProtector protector) : base(s => protector.Protect(s), s => protector.Unprotect(s), default)
            { }
        }

        private void IdentityUserContextBase(ModelBuilder builder)
        {
            StoreOptions storeOptions = this.GetService<IDbContextOptions>()
                            .Extensions.OfType<CoreOptionsExtension>()
                            .FirstOrDefault()?.ApplicationServiceProvider
                            ?.GetService<IOptions<IdentityOptions>>()
                            ?.Value?.Stores;

            var maxKeyLength = storeOptions?.MaxLengthForKeys ?? 0;
            var encryptPersonalData = storeOptions?.ProtectPersonalData ?? false;
            PersonalDataConverter converter = null;

            builder.Entity<MultiTenantUserIdentity>(b =>
            {
                b.HasKey(u => u.Id);
                //b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex");
                b.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");
                //b.ToTable("AspNetUsers");
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                if (encryptPersonalData)
                {
                    converter = new PersonalDataConverter(this.GetService<IPersonalDataProtector>());
                    var personalDataProps = typeof(MultiTenantUserIdentity).GetProperties().Where(
                                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                    foreach (var p in personalDataProps)
                    {
                        if (p.PropertyType != typeof(string))
                        {
                            throw new InvalidOperationException("Can only protect strings");
                        }
                        b.Property(typeof(string), p.Name).HasConversion(converter);
                    }
                }

                b.HasMany<UserIdentityUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany<UserIdentityUserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
                b.HasMany<UserIdentityUserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
            });

            builder.Entity<UserIdentityUserClaim>(b =>
            {
                b.HasKey(uc => uc.Id);
                //b.ToTable("AspNetUserClaims");
            });

            builder.Entity<UserIdentityUserLogin>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                if (maxKeyLength > 0)
                {
                    b.Property(l => l.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(l => l.ProviderKey).HasMaxLength(maxKeyLength);
                }

                //b.ToTable("AspNetUserLogins");
            });

            builder.Entity<UserIdentityUserToken>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                if (maxKeyLength > 0)
                {
                    b.Property(t => t.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(t => t.Name).HasMaxLength(maxKeyLength);
                }

                if (encryptPersonalData)
                {
                    var tokenProps = typeof(UserIdentityUserToken).GetProperties()
                        .Where(prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                    foreach (var p in tokenProps)
                    {
                        if (p.PropertyType != typeof(string))
                        {
                            throw new InvalidOperationException("Can only protect strings");
                        }
                        b.Property(typeof(string), p.Name).HasConversion(converter);
                    }
                }

                //b.ToTable("AspNetUserTokens");
            });
        }

        private void IdentityDbContextBase(ModelBuilder builder)
        {
            builder.Entity<MultiTenantUserIdentity>(b =>
            {
                b.HasMany<UserIdentityUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
            });

            builder.Entity<UserIdentityRole>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
                //b.ToTable("AspNetRoles");
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<UserIdentityUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<UserIdentityRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<UserIdentityRoleClaim>(b =>
            {
                b.HasKey(rc => rc.Id);
                //b.ToTable("AspNetRoleClaims");
            });

            builder.Entity<UserIdentityUserRole>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                //b.ToTable("AspNetUserRoles");
            });
        }

        private void ConfigureIdentityContext(ModelBuilder builder)
        {
            builder.Entity<MultiTenantUserIdentity>().HasIndex(i => new { i.TenantId, i.UserName }).HasName("TenantUserNameIndex").IsUnique();
            builder.Entity<MultiTenantUserIdentity>().HasIndex(i => new { i.TenantId, i.Email }).HasName("TenantEmailIndex").IsUnique();

            builder.Entity<UserIdentityRole>().ToTable(TableConsts.IdentityRoles);
            builder.Entity<UserIdentityRoleClaim>().ToTable(TableConsts.IdentityRoleClaims);
            builder.Entity<UserIdentityUserRole>().ToTable(TableConsts.IdentityUserRoles);

            builder.Entity<MultiTenantUserIdentity>().ToTable(TableConsts.IdentityUsers);
            builder.Entity<UserIdentityUserLogin>().ToTable(TableConsts.IdentityUserLogins);
            builder.Entity<UserIdentityUserClaim>().ToTable(TableConsts.IdentityUserClaims);
            builder.Entity<UserIdentityUserToken>().ToTable(TableConsts.IdentityUserTokens);

            builder.Entity<Tenant>().ToTable(TableConsts.Tenants);
        }
    }
}