using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class Auth01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isonline",
                table: "accounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isonline",
                table: "accounts");
        }
    }
}
