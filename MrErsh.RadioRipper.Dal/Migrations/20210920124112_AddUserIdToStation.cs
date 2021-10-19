using Microsoft.EntityFrameworkCore.Migrations;

namespace MrErsh.RadioRipper.Dal.Migrations
{
    public partial class AddUserIdToStation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Stations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_UserId",
                table: "Stations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stations_UserId",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Stations");
        }
    }
}
