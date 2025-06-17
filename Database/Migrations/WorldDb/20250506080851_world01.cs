using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.WorldDb
{
    /// <inheritdoc />
    public partial class world01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "channels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "channels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    channelid = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    channelport = table.Column<int>(type: "int", nullable: false),
                    channelstatus = table.Column<int>(type: "int", nullable: false),
                    ipaddress = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    maxplayers = table.Column<int>(type: "int", nullable: false),
                    onlineplayers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channels", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
