using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class Auth04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnderMaintenance",
                table: "servers",
                newName: "undermaintenance");

            migrationBuilder.RenameColumn(
                name: "ServerName",
                table: "servers",
                newName: "servername");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "servers",
                newName: "serverid");

            migrationBuilder.RenameColumn(
                name: "Port",
                table: "servers",
                newName: "port");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "servers",
                newName: "ipaddress");

            migrationBuilder.RenameColumn(
                name: "Channels",
                table: "servers",
                newName: "channels");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "servers",
                newName: "id");

            migrationBuilder.AddColumn<byte>(
                name: "serverid",
                table: "channels",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "serverid",
                table: "channels");

            migrationBuilder.RenameColumn(
                name: "undermaintenance",
                table: "servers",
                newName: "UnderMaintenance");

            migrationBuilder.RenameColumn(
                name: "servername",
                table: "servers",
                newName: "ServerName");

            migrationBuilder.RenameColumn(
                name: "serverid",
                table: "servers",
                newName: "ServerId");

            migrationBuilder.RenameColumn(
                name: "port",
                table: "servers",
                newName: "Port");

            migrationBuilder.RenameColumn(
                name: "ipaddress",
                table: "servers",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "channels",
                table: "servers",
                newName: "Channels");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "servers",
                newName: "Id");
        }
    }
}
