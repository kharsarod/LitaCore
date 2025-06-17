using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.WorldDb
{
    /// <inheritdoc />
    public partial class Buff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "buffs",
                columns: table => new
                {
                    buffid = table.Column<short>(type: "smallint", nullable: false),
                    bufftype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    effectid = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    activationchance = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    activationdelayms = table.Column<int>(type: "int", nullable: false),
                    durationms = table.Column<int>(type: "int", nullable: false),
                    expirationbuffid = table.Column<short>(type: "smallint", nullable: false),
                    expirationbuffchance = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buffs", x => x.buffid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "buffs_translations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    language = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    buffdatabuffid = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buffs_translations", x => x.id);
                    table.ForeignKey(
                        name: "FK_buffs_translations_buffs_buffdatabuffid",
                        column: x => x.buffdatabuffid,
                        principalTable: "buffs",
                        principalColumn: "buffid");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_buffs_translations_buffdatabuffid",
                table: "buffs_translations",
                column: "buffdatabuffid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "buffs_translations");

            migrationBuilder.DropTable(
                name: "buffs");
        }
    }
}
