using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveSomeColumnInWarehouseReceiving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalStock",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "UpdatedStock",
                table: "WarehouseReceived");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalStock",
                table: "WarehouseReceived",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UpdatedStock",
                table: "WarehouseReceived",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
