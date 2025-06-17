using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class litacore07_account_ip_lastlogout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bcard_items_itemid",
                table: "bcard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bcard",
                table: "bcard");

            migrationBuilder.RenameTable(
                name: "bcard",
                newName: "bcards");

            migrationBuilder.RenameIndex(
                name: "IX_bcard_itemid",
                table: "bcards",
                newName: "IX_bcards_itemid");

            migrationBuilder.AddColumn<string>(
                name: "ipaddress",
                table: "accounts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "language",
                table: "accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "lastlogin",
                table: "accounts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_bcards",
                table: "bcards",
                column: "bcardid");

            migrationBuilder.AddForeignKey(
                name: "FK_bcards_items_itemid",
                table: "bcards",
                column: "itemid",
                principalTable: "items",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bcards_items_itemid",
                table: "bcards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bcards",
                table: "bcards");

            migrationBuilder.DropColumn(
                name: "ipaddress",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "language",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "lastlogin",
                table: "accounts");

            migrationBuilder.RenameTable(
                name: "bcards",
                newName: "bcard");

            migrationBuilder.RenameIndex(
                name: "IX_bcards_itemid",
                table: "bcard",
                newName: "IX_bcard_itemid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bcard",
                table: "bcard",
                column: "bcardid");

            migrationBuilder.AddForeignKey(
                name: "FK_bcard_items_itemid",
                table: "bcard",
                column: "itemid",
                principalTable: "items",
                principalColumn: "id");
        }
    }
}
