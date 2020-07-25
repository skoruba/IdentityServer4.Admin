using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.Migrations.Identity
{
    public partial class DeleteTenantFromIdentityContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserLogins");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserClaims");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RoleClaims");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "UserTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "UserRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "UserLogins",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "UserClaims",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "RoleClaims",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
