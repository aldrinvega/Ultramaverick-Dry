using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustLabtestEntity5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseReceiving",
                table: "LabTestRequests",
                newName: "WarehouseReceivingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseReceivingId",
                table: "LabTestRequests",
                newName: "WarehouseReceiving");
        }
    }
}
