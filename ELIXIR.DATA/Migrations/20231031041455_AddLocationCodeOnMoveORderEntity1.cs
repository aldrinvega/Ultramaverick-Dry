using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddLocationCodeOnMoveORderEntity1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountTitleCode",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitleCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitleCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountTitleCode",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "AccountTitleCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "AccountTitleCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "MiscellaneousIssues");
        }
    }
}
