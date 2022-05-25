using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveWarehouseStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchCount",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "TransformId",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "WarehouseItemStatus",
                table: "WarehouseReceived");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatchCount",
                table: "WarehouseReceived",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransformId",
                table: "WarehouseReceived",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WarehouseItemStatus",
                table: "WarehouseReceived",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
