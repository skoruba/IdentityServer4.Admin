﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;

namespace SkorubaIdentityServer4Admin.Admin.Data.Migrations
{
    [DbContext(typeof(AdminDbContext))]
    partial class AdminDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiResource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<string>("DisplayName")
                        .HasMaxLength(200);

                    b.Property<bool>("Enabled");

                    b.Property<DateTime?>("LastAccessed");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool>("NonEditable");

                    b.Property<DateTime?>("Updated");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ApiResources");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiResourceClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ApiResourceId");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ApiResourceId");

                    b.ToTable("ApiClaims");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiResourceProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ApiResourceId");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ApiResourceId");

                    b.ToTable("ApiProperties");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiScope", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ApiResourceId");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<string>("DisplayName")
                        .HasMaxLength(200);

                    b.Property<bool>("Emphasize");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool>("Required");

                    b.Property<bool>("ShowInDiscoveryDocument");

                    b.HasKey("Id");

                    b.HasIndex("ApiResourceId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ApiScopes");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiScopeClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ApiScopeId");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ApiScopeId");

                    b.ToTable("ApiScopeClaims");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiSecret", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ApiResourceId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<DateTime?>("Expiration");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(4000);

                    b.HasKey("Id");

                    b.HasIndex("ApiResourceId");

                    b.ToTable("ApiSecrets");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AbsoluteRefreshTokenLifetime");

                    b.Property<int>("AccessTokenLifetime");

                    b.Property<int>("AccessTokenType");

                    b.Property<bool>("AllowAccessTokensViaBrowser");

                    b.Property<bool>("AllowOfflineAccess");

                    b.Property<bool>("AllowPlainTextPkce");

                    b.Property<bool>("AllowRememberConsent");

                    b.Property<bool>("AlwaysIncludeUserClaimsInIdToken");

                    b.Property<bool>("AlwaysSendClientClaims");

                    b.Property<int>("AuthorizationCodeLifetime");

                    b.Property<bool>("BackChannelLogoutSessionRequired");

                    b.Property<string>("BackChannelLogoutUri")
                        .HasMaxLength(2000);

                    b.Property<string>("ClientClaimsPrefix")
                        .HasMaxLength(200);

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("ClientName")
                        .HasMaxLength(200);

                    b.Property<string>("ClientUri")
                        .HasMaxLength(2000);

                    b.Property<int?>("ConsentLifetime");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<int>("DeviceCodeLifetime");

                    b.Property<bool>("EnableLocalLogin");

                    b.Property<bool>("Enabled");

                    b.Property<bool>("FrontChannelLogoutSessionRequired");

                    b.Property<string>("FrontChannelLogoutUri")
                        .HasMaxLength(2000);

                    b.Property<int>("IdentityTokenLifetime");

                    b.Property<bool>("IncludeJwtId");

                    b.Property<DateTime?>("LastAccessed");

                    b.Property<string>("LogoUri")
                        .HasMaxLength(2000);

                    b.Property<bool>("NonEditable");

                    b.Property<string>("PairWiseSubjectSalt")
                        .HasMaxLength(200);

                    b.Property<string>("ProtocolType")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("RefreshTokenExpiration");

                    b.Property<int>("RefreshTokenUsage");

                    b.Property<bool>("RequireClientSecret");

                    b.Property<bool>("RequireConsent");

                    b.Property<bool>("RequirePkce");

                    b.Property<int>("SlidingRefreshTokenLifetime");

                    b.Property<bool>("UpdateAccessTokenClaimsOnRefresh");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UserCodeType")
                        .HasMaxLength(100);

                    b.Property<int?>("UserSsoLifetime");

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientClaims");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientCorsOrigin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("Origin")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientCorsOrigins");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientGrantType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("GrantType")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientGrantTypes");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientIdPRestriction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientIdPRestrictions");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("PostLogoutRedirectUri")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientPostLogoutRedirectUris");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientProperties");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientRedirectUri", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("RedirectUri")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientRedirectUris");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientScope", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<string>("Scope")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientScopes");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientSecret", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<DateTime?>("Expiration");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(4000);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientSecrets");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.DeviceFlowCodes", b =>
                {
                    b.Property<string>("UserCode")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(200);

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(50000);

                    b.Property<string>("DeviceCode")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime?>("Expiration")
                        .IsRequired();

                    b.Property<string>("SubjectId")
                        .HasMaxLength(200);

                    b.HasKey("UserCode");

                    b.HasIndex("DeviceCode")
                        .IsUnique();

                    b.ToTable("DeviceCodes");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.IdentityClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IdentityResourceId");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("IdentityResourceId");

                    b.ToTable("IdentityClaims");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.IdentityResource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<string>("DisplayName")
                        .HasMaxLength(200);

                    b.Property<bool>("Emphasize");

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool>("NonEditable");

                    b.Property<bool>("Required");

                    b.Property<bool>("ShowInDiscoveryDocument");

                    b.Property<DateTime?>("Updated");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("IdentityResources");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.IdentityResourceProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IdentityResourceId");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("IdentityResourceId");

                    b.ToTable("IdentityProperties");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.PersistedGrant", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(200);

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(50000);

                    b.Property<DateTime?>("Expiration");

                    b.Property<string>("SubjectId")
                        .HasMaxLength(200);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Key");

                    b.HasIndex("SubjectId", "ClientId", "Type");

                    b.ToTable("PersistedGrants");
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Entities.Log", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Exception");

                    b.Property<string>("Level")
                        .HasMaxLength(128);

                    b.Property<string>("LogEvent");

                    b.Property<string>("Message");

                    b.Property<string>("MessageTemplate");

                    b.Property<string>("Properties")
                        .HasColumnType("xml");

                    b.Property<DateTimeOffset>("TimeStamp");

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityRoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityUserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityUserLogin", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityUserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityUserToken", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiResourceClaim", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.ApiResource", "ApiResource")
                        .WithMany("UserClaims")
                        .HasForeignKey("ApiResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiResourceProperty", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.ApiResource", "ApiResource")
                        .WithMany("Properties")
                        .HasForeignKey("ApiResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiScope", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.ApiResource", "ApiResource")
                        .WithMany("Scopes")
                        .HasForeignKey("ApiResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiScopeClaim", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.ApiScope", "ApiScope")
                        .WithMany("UserClaims")
                        .HasForeignKey("ApiScopeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ApiSecret", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.ApiResource", "ApiResource")
                        .WithMany("Secrets")
                        .HasForeignKey("ApiResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientClaim", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("Claims")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientCorsOrigin", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("AllowedCorsOrigins")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientGrantType", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("AllowedGrantTypes")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientIdPRestriction", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("IdentityProviderRestrictions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("PostLogoutRedirectUris")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientProperty", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("Properties")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientRedirectUri", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("RedirectUris")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientScope", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("AllowedScopes")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.ClientSecret", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.Client", "Client")
                        .WithMany("ClientSecrets")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.IdentityClaim", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.IdentityResource", "IdentityResource")
                        .WithMany("UserClaims")
                        .HasForeignKey("IdentityResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.EntityFramework.Entities.IdentityResourceProperty", b =>
                {
                    b.HasOne("IdentityServer4.EntityFramework.Entities.IdentityResource", "IdentityResource")
                        .WithMany("Properties")
                        .HasForeignKey("IdentityResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityRoleClaim", b =>
                {
                    b.HasOne("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityUserClaim", b =>
                {
                    b.HasOne("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityUserLogin", b =>
                {
                    b.HasOne("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityUserRole", b =>
                {
                    b.HasOne("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentityUserToken", b =>
                {
                    b.HasOne("Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity.UserIdentity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
