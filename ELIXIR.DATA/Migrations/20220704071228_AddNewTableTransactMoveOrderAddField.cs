using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewTableTransactMoveOrderAddField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrepared",
                table: "TransactMoveOrder");

            migrationBuilder.AddColumn<bool>(
                name: "IsTransact",
                table: "TransactMoveOrder",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTransact",
                table: "TransactMoveOrder");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrepared",
                table: "TransactMoveOrder",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
