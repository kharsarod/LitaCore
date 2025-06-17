using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations.CharacterDb
{
    /// <inheritdoc />
    public partial class ActionBar03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actionbarid",
                table: "actionbars");

            migrationBuilder.DropColumn(
                name: "invslot",
                table: "actionbars");

            migrationBuilder.RenameColumn(
                name: "objectid",
                table: "actionbars",
                newName: "morph");

            migrationBuilder.AlterColumn<short>(
                name: "slot",
                table: "actionbars",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "characterid",
                table: "actionbars",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddColumn<short>(
                name: "pos",
                table: "actionbars",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "q1",
                table: "actionbars",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "q2",
                table: "actionbars",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "type",
                table: "actionbars",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pos",
                table: "actionbars");

            migrationBuilder.DropColumn(
                name: "q1",
                table: "actionbars");

            migrationBuilder.DropColumn(
                name: "q2",
                table: "actionbars");

            migrationBuilder.DropColumn(
                name: "type",
                table: "actionbars");

            migrationBuilder.RenameColumn(
                name: "morph",
                table: "actionbars",
                newName: "objectid");

            migrationBuilder.AlterColumn<int>(
                name: "slot",
                table: "actionbars",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<short>(
                name: "characterid",
                table: "actionbars",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<byte>(
                name: "actionbarid",
                table: "actionbars",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "invslot",
                table: "actionbars",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
