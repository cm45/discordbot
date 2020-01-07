using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.EscapeFromTarkovAPI.Migrations
{
    public partial class addeddetailstoammonitionmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Damage",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "PenetrationValue",
                table: "Ammos");

            migrationBuilder.AddColumn<int>(
                name: "AccuracyPercentage",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<int>(
                name: "FleshDamage",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FragmentationChancePercentage",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PenetrationPower",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Projectiles",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Recoil",
                table: "Ammos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccuracyPercentage",
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
                name: "FleshDamage",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "FragmentationChancePercentage",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "PenetrationPower",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "Projectiles",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "Recoil",
                table: "Ammos");

            migrationBuilder.AddColumn<float>(
                name: "Damage",
                table: "Ammos",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PenetrationValue",
                table: "Ammos",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
