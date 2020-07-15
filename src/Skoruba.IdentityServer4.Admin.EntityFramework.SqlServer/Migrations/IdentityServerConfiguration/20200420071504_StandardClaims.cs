using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.Migrations.IdentityServerConfiguration
{
    public partial class StandardClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StandardClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(nullable: false),
                    LastUsedTimestamp = table.Column<DateTimeOffset>(nullable: true),
                    UseCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardClaims", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StandardClaims_ClaimType",
                table: "StandardClaims",
                column: "ClaimType",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandardClaims");
        }
    }
}
