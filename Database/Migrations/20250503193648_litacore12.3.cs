using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class litacore123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "closedefence",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "critluckrate",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "critrate",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "defdodge",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "distdefence",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "fairylevel",
                table: "character_items",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "fairymonsterremaining",
                table: "character_items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "hitrate",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "magicdefence",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "maxdmg",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "mindmg",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closedefence",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "critluckrate",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "critrate",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "defdodge",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "distdefence",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "fairylevel",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "fairymonsterremaining",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "hitrate",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "magicdefence",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "maxdmg",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "mindmg",
                table: "character_items");
        }
    }
}
