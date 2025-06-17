using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class Auth07 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ipaddress",
                table: "servers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ipaddress",
                table: "servers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
