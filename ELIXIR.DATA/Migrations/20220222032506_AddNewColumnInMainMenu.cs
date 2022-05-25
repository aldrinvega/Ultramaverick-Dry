using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewColumnInMainMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuPath",
                table: "Modules");

            migrationBuilder.AddColumn<string>(
                name: "MenuPath",
                table: "MainMenus",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuPath",
                table: "MainMenus");

            migrationBuilder.AddColumn<string>(
                name: "MenuPath",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
