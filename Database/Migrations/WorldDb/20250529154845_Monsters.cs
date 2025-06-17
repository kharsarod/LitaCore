using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.WorldDb
{
    /// <inheritdoc />
    public partial class Monsters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "monsters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    monsterid = table.Column<int>(type: "int", nullable: false),
                    vnum = table.Column<short>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mapid = table.Column<short>(type: "smallint", nullable: false),
                    mapx = table.Column<short>(type: "smallint", nullable: false),
                    mapy = table.Column<short>(type: "smallint", nullable: false),
                    ismoving = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    position = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monsters", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "monsters");
        }
    }
}
