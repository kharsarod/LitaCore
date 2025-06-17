using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class litacore12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isequipped",
                table: "character_items");

            migrationBuilder.AddColumn<int>(
                name: "equipmentslot",
                table: "character_items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "equipmentslot",
                table: "character_items");

            migrationBuilder.AddColumn<bool>(
                name: "isequipped",
                table: "character_items",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
