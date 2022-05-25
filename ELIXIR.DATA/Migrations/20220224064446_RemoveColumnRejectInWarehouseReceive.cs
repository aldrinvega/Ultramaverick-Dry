using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveColumnRejectInWarehouseReceive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualReject",
                table: "WarehouseReceived");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualReject",
                table: "WarehouseReceived",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
