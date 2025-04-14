using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class litacore06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    defaultupgrade = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    cellonlevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    requiredclass = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    closedefence = table.Column<short>(type: "smallint", nullable: false),
                    color = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    concentrate = table.Column<short>(type: "smallint", nullable: false),
                    criticalluckrate = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    criticalrate = table.Column<short>(type: "smallint", nullable: false),
                    damagemaximum = table.Column<short>(type: "smallint", nullable: false),
                    damageminimum = table.Column<short>(type: "smallint", nullable: false),
                    darkelement = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    darkresistance = table.Column<short>(type: "smallint", nullable: false),
                    defencedodge = table.Column<short>(type: "smallint", nullable: false),
                    distancedefence = table.Column<short>(type: "smallint", nullable: false),
                    distancedefencedodge = table.Column<short>(type: "smallint", nullable: false),
                    effect = table.Column<short>(type: "smallint", nullable: false),
                    effectdata = table.Column<int>(type: "int", nullable: false),
                    element = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    elementrate = table.Column<short>(type: "smallint", nullable: false),
                    equipmentslot = table.Column<int>(type: "int", nullable: false),
                    fireelement = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    fireresistance = table.Column<short>(type: "smallint", nullable: false),
                    height = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    hitrate = table.Column<short>(type: "smallint", nullable: false),
                    hp = table.Column<short>(type: "smallint", nullable: false),
                    hpregeneration = table.Column<short>(type: "smallint", nullable: false),
                    isblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    iscolored = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isconsumable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isdroppable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isheroic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isholder = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isminilandobject = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    issoldable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    istradable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    itemsubtype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    itemtype = table.Column<int>(type: "int", nullable: false),
                    itemvalidtime = table.Column<long>(type: "bigint", nullable: false),
                    leveljobminimum = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    levelminimum = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    lightelement = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    lightresistance = table.Column<short>(type: "smallint", nullable: false),
                    magicdefence = table.Column<short>(type: "smallint", nullable: false),
                    maxcellon = table.Column<short>(type: "smallint", nullable: false),
                    maxcellonlvl = table.Column<short>(type: "smallint", nullable: false),
                    maxelementrate = table.Column<short>(type: "smallint", nullable: false),
                    maximumammo = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    minilandobjectpoint = table.Column<int>(type: "int", nullable: false),
                    morehp = table.Column<short>(type: "smallint", nullable: false),
                    moremp = table.Column<short>(type: "smallint", nullable: false),
                    morph = table.Column<short>(type: "smallint", nullable: false),
                    mp = table.Column<short>(type: "smallint", nullable: false),
                    mpregeneration = table.Column<short>(type: "smallint", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false),
                    selltonpcprice = table.Column<long>(type: "bigint", nullable: false),
                    pvpdefence = table.Column<short>(type: "smallint", nullable: false),
                    pvpstrength = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    reduceoposantresistance = table.Column<short>(type: "smallint", nullable: false),
                    reputationminimum = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    reputprice = table.Column<long>(type: "bigint", nullable: false),
                    secondaryelement = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    sex = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    speed = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    sptype = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    waitdelay = table.Column<short>(type: "smallint", nullable: false),
                    waterelement = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    waterresistance = table.Column<short>(type: "smallint", nullable: false),
                    width = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "items_translations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    language = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    itemid = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items_translations", x => x.id);
                    table.ForeignKey(
                        name: "FK_items_translations_items_itemid",
                        column: x => x.itemid,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_items_translations_itemid",
                table: "items_translations",
                column: "itemid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "items_translations");

            migrationBuilder.DropTable(
                name: "items");
        }
    }
}
