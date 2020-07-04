using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.Migrations.Tenant
{
    public partial class Edition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EditionId",
                table: "Tenants",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Edition",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edition", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_EditionId",
                table: "Tenants",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Edition_Name",
                table: "Edition",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Edition_EditionId",
                table: "Tenants",
                column: "EditionId",
                principalTable: "Edition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Edition_EditionId",
                table: "Tenants");

            migrationBuilder.DropTable(
                name: "Edition");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_EditionId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "EditionId",
                table: "Tenants");
        }
    }
}
