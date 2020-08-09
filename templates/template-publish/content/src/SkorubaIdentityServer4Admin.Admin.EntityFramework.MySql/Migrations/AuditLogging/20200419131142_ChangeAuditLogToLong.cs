using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SkorubaIdentityServer4Admin.Admin.EntityFramework.MySql.Migrations.AuditLogging
{
    public partial class ChangeAuditLogToLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_AuditLog", "AuditLog");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AuditLog",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey("PK_AuditLog", "AuditLog", "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_AuditLog", "AuditLog");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AuditLog",
                type: "int",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey("PK_AuditLog", "AuditLog", "Id");
        }
    }
}






