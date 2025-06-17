using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.WorldDb
{
    /// <inheritdoc />
    public partial class MonstersAndDrops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VNum",
                table: "bcards");

            migrationBuilder.AddColumn<int>(
                name: "dialogid",
                table: "npcs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "npcmonstervnum",
                table: "bcards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "drops",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    dropid = table.Column<short>(type: "smallint", nullable: false),
                    vnum = table.Column<short>(type: "smallint", nullable: false),
                    amount = table.Column<int>(type: "int", nullable: false),
                    chance = table.Column<int>(type: "int", nullable: false),
                    monstervnum = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drops", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "npc_monster_skills",
                columns: table => new
                {
                    npcmonsterskillid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    vnum = table.Column<short>(type: "smallint", nullable: false),
                    rate = table.Column<short>(type: "smallint", nullable: false),
                    skillvnum = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_npc_monster_skills", x => x.npcmonsterskillid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "npc_monsters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    vnum = table.Column<short>(type: "smallint", nullable: false),
                    itemamountrequired = table.Column<short>(type: "smallint", nullable: false),
                    attackclass = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    attackupgrade = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    basicarea = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    basiccooldown = table.Column<short>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    basicrange = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    basicskill = table.Column<short>(type: "smallint", nullable: false),
                    @catch = table.Column<bool>(name: "catch", type: "tinyint(1)", nullable: false),
                    closedefence = table.Column<short>(type: "smallint", nullable: false),
                    concentrate = table.Column<short>(type: "smallint", nullable: false),
                    criticalchance = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    criticalrate = table.Column<short>(type: "smallint", nullable: false),
                    damagemaximum = table.Column<short>(type: "smallint", nullable: false),
                    damageminimum = table.Column<short>(type: "smallint", nullable: false),
                    darkresistance = table.Column<short>(type: "smallint", nullable: false),
                    defencedodge = table.Column<short>(type: "smallint", nullable: false),
                    defenceupgrade = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    distancedefence = table.Column<short>(type: "smallint", nullable: false),
                    distancedefencedodge = table.Column<short>(type: "smallint", nullable: false),
                    element = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    elementrate = table.Column<short>(type: "smallint", nullable: false),
                    fireresistance = table.Column<short>(type: "smallint", nullable: false),
                    herolevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    heroxp = table.Column<int>(type: "int", nullable: false),
                    ishostile = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    jobxp = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    lightresistance = table.Column<short>(type: "smallint", nullable: false),
                    magicdefence = table.Column<short>(type: "smallint", nullable: false),
                    maxhp = table.Column<int>(type: "int", nullable: false),
                    maxmp = table.Column<int>(type: "int", nullable: false),
                    monstertype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    noaggresiveicon = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    noticerange = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    originalvnum = table.Column<short>(type: "smallint", nullable: false),
                    race = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    racetype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    respawntime = table.Column<int>(type: "int", nullable: false),
                    speed = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    vnumrequired = table.Column<short>(type: "smallint", nullable: false),
                    waterresistance = table.Column<short>(type: "smallint", nullable: false),
                    xp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_npc_monsters", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shop_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    itemid = table.Column<short>(type: "smallint", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false),
                    rarity = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    upgrade = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    slot = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    color = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    window = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    shopid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_items", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shop_translations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopid = table.Column<int>(type: "int", nullable: false),
                    language = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_translations", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shops",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    npcid = table.Column<int>(type: "int", nullable: false),
                    menutype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    shoptype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    shopid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shops", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "drops");

            migrationBuilder.DropTable(
                name: "npc_monster_skills");

            migrationBuilder.DropTable(
                name: "npc_monsters");

            migrationBuilder.DropTable(
                name: "shop_items");

            migrationBuilder.DropTable(
                name: "shop_translations");

            migrationBuilder.DropTable(
                name: "shops");

            migrationBuilder.DropColumn(
                name: "dialogid",
                table: "npcs");

            migrationBuilder.DropColumn(
                name: "npcmonstervnum",
                table: "bcards");

            migrationBuilder.AddColumn<short>(
                name: "VNum",
                table: "bcards",
                type: "smallint",
                nullable: true);
        }
    }
}
