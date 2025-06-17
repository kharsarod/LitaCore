using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.WorldDb
{
    /// <inheritdoc />
    public partial class Skills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "combos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    skillvnum = table.Column<short>(type: "smallint", nullable: false),
                    animation = table.Column<short>(type: "smallint", nullable: false),
                    effect = table.Column<short>(type: "smallint", nullable: false),
                    hit = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_combos", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    skillvnum = table.Column<short>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<int>(type: "int", nullable: false),
                    itemvnum = table.Column<short>(type: "smallint", nullable: false),
                    @class = table.Column<byte>(name: "class", type: "tinyint unsigned", nullable: false),
                    level = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    levelminimum = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    minimumadventurerlevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    minimumarcherlevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    minimummagicianlevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    minimumswordmanlevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    attackanimation = table.Column<short>(type: "smallint", nullable: false),
                    castanimation = table.Column<short>(type: "smallint", nullable: false),
                    casteffect = table.Column<short>(type: "smallint", nullable: false),
                    castid = table.Column<short>(type: "smallint", nullable: false),
                    casttime = table.Column<short>(type: "smallint", nullable: false),
                    cpcost = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    mpcost = table.Column<short>(type: "smallint", nullable: false),
                    cooldown = table.Column<short>(type: "smallint", nullable: false),
                    duration = table.Column<short>(type: "smallint", nullable: false),
                    effect = table.Column<short>(type: "smallint", nullable: false),
                    element = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    hittype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    range = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    skilltype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    targetrange = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    targettype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    upgradeskill = table.Column<short>(type: "smallint", nullable: false),
                    upgradetype = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "combos");

            migrationBuilder.DropTable(
                name: "skills");
        }
    }
}
