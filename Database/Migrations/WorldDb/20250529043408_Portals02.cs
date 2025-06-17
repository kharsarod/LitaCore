using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.WorldDb
{
    /// <inheritdoc />
    public partial class Portals02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "disabled",
                table: "portals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<sbyte>(
                name: "type",
                table: "portals",
                type: "tinyint",
                nullable: false,
                defaultValue: (sbyte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "disabled",
                table: "portals");

            migrationBuilder.DropColumn(
                name: "type",
                table: "portals");
        }
    }
}
