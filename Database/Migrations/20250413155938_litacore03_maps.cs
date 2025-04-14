using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class litacore03_maps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maps",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bgm = table.Column<int>(type: "int", nullable: false),
                    isshopallowed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    mapgrid = table.Column<byte[]>(type: "longblob", nullable: false),
                    exprate = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    goldrate = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    droprate = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ispvpallowed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    height = table.Column<short>(type: "smallint", nullable: false),
                    width = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maps", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "maps");
        }
    }
}
