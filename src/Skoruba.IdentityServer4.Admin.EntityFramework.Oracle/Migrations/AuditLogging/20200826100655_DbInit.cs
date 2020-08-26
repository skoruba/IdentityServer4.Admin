using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Oracle.EntityFrameworkCore.Metadata;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Oracle.Migrations.AuditLogging
{
    public partial class DbInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn),
                    Event = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    SubjectIdentifier = table.Column<string>(nullable: true),
                    SubjectName = table.Column<string>(nullable: true),
                    SubjectType = table.Column<string>(nullable: true),
                    SubjectAdditionalData = table.Column<string>(maxLength: 50000, nullable: true),
                    Action = table.Column<string>(nullable: true),
                    Data = table.Column<string>(maxLength: 50000, nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
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
