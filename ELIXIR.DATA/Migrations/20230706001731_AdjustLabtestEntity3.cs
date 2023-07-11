using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustLabtestEntity3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseReceivedId",
                table: "LabTestRequests",
                newName: "WarehouseReceiving");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseReceiving",
                table: "LabTestRequests",
                newName: "WarehouseReceivedId");
        }
    }
}
