using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.EscapeFromTarkovAPI.Migrations
{
    public partial class Removedammosfromweaponmodelagainremovedarmoreffectivenessfromammomodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ammos_Weapons_WeaponId",
                table: "Ammos");

            migrationBuilder.DropIndex(
                name: "IX_Ammos_WeaponId",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "ArmorEffectivenessAgainst1",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "ArmorEffectivenessAgainst2",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "ArmorEffectivenessAgainst3",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "ArmorEffectivenessAgainst4",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "ArmorEffectivenessAgainst5",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "ArmorEffectivenessAgainst6",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "WeaponId",
                table: "Ammos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst1",
                table: "Ammos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst2",
                table: "Ammos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst3",
                table: "Ammos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst4",
                table: "Ammos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst5",
                table: "Ammos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst6",
                table: "Ammos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeaponId",
                table: "Ammos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ammos_WeaponId",
                table: "Ammos",
                column: "WeaponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ammos_Weapons_WeaponId",
                table: "Ammos",
                column: "WeaponId",
                principalTable: "Weapons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
