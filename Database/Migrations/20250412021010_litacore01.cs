using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class litacore01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rank = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    isbanned = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    accountid = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    level = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    exp = table.Column<long>(type: "bigint", nullable: false),
                    joblevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    jobexp = table.Column<long>(type: "bigint", nullable: false),
                    herolevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    heroexp = table.Column<long>(type: "bigint", nullable: false),
                    @class = table.Column<byte>(name: "class", type: "tinyint unsigned", nullable: false),
                    gender = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    haircolor = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    hairstyle = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    health = table.Column<int>(type: "int", nullable: false),
                    maxhealth = table.Column<int>(type: "int", nullable: false),
                    mana = table.Column<int>(type: "int", nullable: false),
                    maxmana = table.Column<int>(type: "int", nullable: false),
                    dignity = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    reputation = table.Column<int>(type: "int", nullable: false),
                    gold = table.Column<long>(type: "bigint", nullable: false),
                    compliments = table.Column<short>(type: "smallint", nullable: false),
                    mapid = table.Column<short>(type: "smallint", nullable: false),
                    mapposx = table.Column<short>(type: "smallint", nullable: false),
                    mapposy = table.Column<short>(type: "smallint", nullable: false),
                    biography = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slot = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    act4deadcount = table.Column<int>(type: "int", nullable: false),
                    act4victims = table.Column<int>(type: "int", nullable: false),
                    act4points = table.Column<int>(type: "int", nullable: false),
                    isarenachampion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isblockedbuff = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isemoticonblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isexchangeblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isfamilyrequestblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isfriendrequestblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isgrouprequestblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isherochatblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ishealthblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    maxpets = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    isminilandinviteblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    minilandmsg = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    minilandpts = table.Column<short>(type: "smallint", nullable: false),
                    minilandstate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cursoraimlock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isquickgetupblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ragepts = table.Column<long>(type: "bigint", nullable: false),
                    specialistaddpts = table.Column<int>(type: "int", nullable: false),
                    specialistpts = table.Column<int>(type: "int", nullable: false),
                    state = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    talentarenaloses = table.Column<int>(type: "int", nullable: false),
                    talentarenasurrender = table.Column<int>(type: "int", nullable: false),
                    talentarenawins = table.Column<int>(type: "int", nullable: false),
                    iswhispblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isdisplayhealthblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isdisplaycdblocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isblockedhud = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isblockedhat = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "characters");
        }
    }
}
