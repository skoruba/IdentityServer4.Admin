using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Sqlite.Migrations.AuditLogging
{
    public partial class DbInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Event = table.Column<string>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    SubjectIdentifier = table.Column<string>(type: "TEXT", nullable: true),
                    SubjectName = table.Column<string>(type: "TEXT", nullable: true),
                    SubjectType = table.Column<string>(type: "TEXT", nullable: true),
                    SubjectAdditionalData = table.Column<string>(type: "TEXT", nullable: true),
                    Action = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");
        }
    }
}
