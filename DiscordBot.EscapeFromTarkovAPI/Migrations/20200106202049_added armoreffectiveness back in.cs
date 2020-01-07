using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.EscapeFromTarkovAPI.Migrations
{
    public partial class addedarmoreffectivenessbackin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst1",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst2",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst3",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst4",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst5",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorEffectivenessAgainst6",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
