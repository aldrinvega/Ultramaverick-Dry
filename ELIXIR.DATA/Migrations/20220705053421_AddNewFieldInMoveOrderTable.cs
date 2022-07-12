using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewFieldInMoveOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReject",
                table: "MoveOrders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectBy",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReject",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "RejectBy",
                table: "MoveOrders");
        }
    }
}
