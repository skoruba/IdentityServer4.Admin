// Original SQL scripts for database migration come from: https://github.com/RockSolidKnowledge/IdentityServer4.Migration.Scripts/blob/master/MSSQL/ConfigurationDbContext.sql
// Modified by Jan Škoruba

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SkorubaIdentityServer4Admin.Admin.EntityFramework.SqlServer.Migrations.IdentityServerConfiguration
{
    public partial class UpdateIdentityServerToVersion4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiClaims_ApiResources_ApiResourceId",
                table: "ApiClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiProperties_ApiResources_ApiResourceId",
                table: "ApiProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ApiScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopes_ApiResources_ApiResourceId",
                table: "ApiScopes");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                table: "IdentityProperties");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopes_ApiResourceId",
                table: "ApiScopes");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ApiScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityProperties",
                table: "IdentityProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiProperties",
                table: "ApiProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiClaims",
                table: "ApiClaims");

            migrationBuilder.RenameTable(
                name: "IdentityProperties",
                newName: "IdentityResourceProperties");

            migrationBuilder.RenameTable(
                name: "ApiProperties",
                newName: "ApiResourceProperties");

            migrationBuilder.RenameTable(
                name: "ApiClaims",
                newName: "ApiResourceClaims");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityProperties_IdentityResourceId",
                table: "IdentityResourceProperties",
                newName: "IX_IdentityResourceProperties_IdentityResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiProperties_ApiResourceId",
                table: "ApiResourceProperties",
                newName: "IX_ApiResourceProperties_ApiResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiClaims_ApiResourceId",
                table: "ApiResourceClaims",
                newName: "IX_ApiResourceClaims_ApiResourceId");

            migrationBuilder.AddColumn<string>(
                name: "AllowedIdentityTokenSigningAlgorithms",
                table: "Clients",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireRequestObject",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "ApiScopes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ScopeId",
                table: "ApiScopeClaims",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AllowedAccessTokenSigningAlgorithms",
                table: "ApiResources",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInDiscoveryDocument",
                table: "ApiResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityResourceProperties",
                table: "IdentityResourceProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiResourceProperties",
                table: "ApiResourceProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiResourceClaims",
                table: "ApiResourceClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApiResourceScopes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(maxLength: 200, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResourceScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiResourceScopes_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiResourceSecrets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Value = table.Column<string>(maxLength: 4000, nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResourceSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiResourceSecrets_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiScopeProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ScopeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiScopeProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiScopeProperties_ApiScopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityResourceClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(maxLength: 200, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityResourceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityResourceClaims_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScopes_ApiResourceId",
                table: "ApiResourceScopes",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceSecrets_ApiResourceId",
                table: "ApiResourceSecrets",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeProperties_ScopeId",
                table: "ApiScopeProperties",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId",
                table: "IdentityResourceClaims",
                column: "IdentityResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceClaims_ApiResources_ApiResourceId",
                table: "ApiResourceClaims",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceProperties_ApiResources_ApiResourceId",
                table: "ApiResourceProperties",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // migrate data

            migrationBuilder.Sql(@"SET IDENTITY_INSERT ApiResourceSecrets ON;  
GO

INSERT INTO ApiResourceSecrets
 (Id, [Description], [Value], Expiration, [Type], Created, ApiResourceId)
SELECT 
 Id, [Description], [Value], Expiration, [Type], Created, ApiResourceId
FROM ApiSecrets
GO

SET IDENTITY_INSERT ApiResourceSecrets OFF;  
GO");

            migrationBuilder.Sql(@"SET IDENTITY_INSERT IdentityResourceClaims ON;  
GO

INSERT INTO IdentityResourceClaims
 (Id, [Type], IdentityResourceId)
SELECT 
 Id, [Type], IdentityResourceId
FROM IdentityClaims
GO
SET IDENTITY_INSERT IdentityResourceClaims OFF; 
GO");

            migrationBuilder.Sql(@"INSERT INTO ApiResourceScopes 
 ([Scope], [ApiResourceId])
SELECT 
 [Name], [ApiResourceId]
FROM ApiScopes");

            migrationBuilder.Sql(@"UPDATE ApiScopeClaims SET ScopeId = ApiScopeId");

            migrationBuilder.Sql(@"UPDATE ApiScopes SET [Enabled] = 1");

            migrationBuilder.DropTable(
                name: "ApiSecrets");

            migrationBuilder.DropTable(
                name: "IdentityClaims");

            migrationBuilder.DropColumn(
                name: "ApiResourceId",
                table: "ApiScopes");

            migrationBuilder.DropColumn(
                name: "ApiScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ScopeId",
                table: "ApiScopeClaims",
                column: "ScopeId",
                principalTable: "ApiScopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityResourceProperties_IdentityResources_IdentityResourceId",
                table: "IdentityResourceProperties",
                column: "IdentityResourceId",
                principalTable: "IdentityResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceClaims_ApiResources_ApiResourceId",
                table: "ApiResourceClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceProperties_ApiResources_ApiResourceId",
                table: "ApiResourceProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityResourceProperties_IdentityResources_IdentityResourceId",
                table: "IdentityResourceProperties");
            
            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityResourceProperties",
                table: "IdentityResourceProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiResourceProperties",
                table: "ApiResourceProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiResourceClaims",
                table: "ApiResourceClaims");

            migrationBuilder.RenameTable(
                name: "IdentityResourceProperties",
                newName: "IdentityProperties");

            migrationBuilder.RenameTable(
                name: "ApiResourceProperties",
                newName: "ApiProperties");

            migrationBuilder.RenameTable(
                name: "ApiResourceClaims",
                newName: "ApiClaims");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId",
                table: "IdentityProperties",
                newName: "IX_IdentityProperties_IdentityResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiResourceProperties_ApiResourceId",
                table: "ApiProperties",
                newName: "IX_ApiProperties_ApiResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiResourceClaims_ApiResourceId",
                table: "ApiClaims",
                newName: "IX_ApiClaims_ApiResourceId");

            migrationBuilder.AddColumn<int>(
                name: "ApiResourceId",
                table: "ApiScopes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApiScopeId",
                table: "ApiScopeClaims",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityProperties",
                table: "IdentityProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiProperties",
                table: "ApiProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiClaims",
                table: "ApiClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApiSecrets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiResourceId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiSecrets_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityResourceId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityClaims_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopes_ApiResourceId",
                table: "ApiScopes",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ApiScopeId",
                table: "ApiScopeClaims",
                column: "ApiScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiSecrets_ApiResourceId",
                table: "ApiSecrets",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityClaims_IdentityResourceId",
                table: "IdentityClaims",
                column: "IdentityResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiClaims_ApiResources_ApiResourceId",
                table: "ApiClaims",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiProperties_ApiResources_ApiResourceId",
                table: "ApiProperties",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Rollback data back
            migrationBuilder.Sql(@"SET IDENTITY_INSERT ApiSecrets ON;  
GO

INSERT INTO ApiSecrets
 (Id, [Description], [Value], Expiration, [Type], Created, ApiResourceId)
SELECT 
 Id, [Description], [Value], Expiration, [Type], Created, ApiResourceId
FROM ApiResourceSecrets
GO

SET IDENTITY_INSERT ApiSecrets OFF;
GO");

            migrationBuilder.Sql(@"SET IDENTITY_INSERT IdentityClaims ON;
GO

INSERT INTO IdentityClaims
 (Id, [Type], IdentityResourceId)
SELECT 
 Id, [Type], IdentityResourceId
FROM IdentityResourceClaims
GO
SET IDENTITY_INSERT IdentityClaims OFF;
GO");

            migrationBuilder.Sql(@"UPDATE asp
SET ApiResourceId = arc.ApiResourceId
FROM ApiScopes asp
    INNER JOIN ApiResourceScopes arc
        ON arc.Id = asp.Id
");

            migrationBuilder.Sql(@"UPDATE ApiScopeClaims SET ApiScopeId = ScopeId");

            migrationBuilder.DropTable(
                name: "ApiResourceScopes");

            migrationBuilder.DropTable(
                name: "ApiResourceSecrets");

            migrationBuilder.DropTable(
                name: "ApiScopeProperties");

            migrationBuilder.DropTable(
                name: "IdentityResourceClaims");

            migrationBuilder.DropColumn(
                name: "AllowedIdentityTokenSigningAlgorithms",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RequireRequestObject",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "ApiScopes");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.DropColumn(
                name: "AllowedAccessTokenSigningAlgorithms",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "ShowInDiscoveryDocument",
                table: "ApiResources");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ApiScopeId",
                table: "ApiScopeClaims",
                column: "ApiScopeId",
                principalTable: "ApiScopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopes_ApiResources_ApiResourceId",
                table: "ApiScopes",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                table: "IdentityProperties",
                column: "IdentityResourceId",
                principalTable: "IdentityResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}








