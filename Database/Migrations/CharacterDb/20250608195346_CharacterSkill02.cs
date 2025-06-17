using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.CharacterDb
{
    /// <inheritdoc />
    public partial class CharacterSkill02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "castid",
                table: "character_skills",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "castid",
                table: "character_skills");
        }
    }
}
