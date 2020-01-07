using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.EscapeFromTarkovAPI.Migrations
{
    public partial class addedammosfieldtoweapons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_Calibers_CaliberId",
                table: "Weapons");

            migrationBuilder.AlterColumn<int>(
                name: "CaliberId",
                table: "Weapons",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "WeaponId",
                table: "Ammos",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_Calibers_CaliberId",
                table: "Weapons",
                column: "CaliberId",
                principalTable: "Calibers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ammos_Weapons_WeaponId",
                table: "Ammos");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_Calibers_CaliberId",
                table: "Weapons");

            migrationBuilder.DropIndex(
                name: "IX_Ammos_WeaponId",
                table: "Ammos");

            migrationBuilder.DropColumn(
                name: "WeaponId",
                table: "Ammos");

            migrationBuilder.AlterColumn<int>(
                name: "CaliberId",
                table: "Weapons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_Calibers_CaliberId",
                table: "Weapons",
                column: "CaliberId",
                principalTable: "Calibers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
