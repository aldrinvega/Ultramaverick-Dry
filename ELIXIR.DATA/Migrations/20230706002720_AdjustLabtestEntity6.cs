using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustLabtestEntity6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WarehouseReceived_LabTestRequestId",
                table: "WarehouseReceived");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseReceived_LabTestRequestId",
                table: "WarehouseReceived",
                column: "LabTestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestRequests_WarehouseReceivingId",
                table: "LabTestRequests",
                column: "WarehouseReceivingId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestRequests_WarehouseReceived_WarehouseReceivingId",
                table: "LabTestRequests",
                column: "WarehouseReceivingId",
                principalTable: "WarehouseReceived",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestRequests_WarehouseReceived_WarehouseReceivingId",
                table: "LabTestRequests");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseReceived_LabTestRequestId",
                table: "WarehouseReceived");

            migrationBuilder.DropIndex(
                name: "IX_LabTestRequests_WarehouseReceivingId",
                table: "LabTestRequests");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseReceived_LabTestRequestId",
                table: "WarehouseReceived",
                column: "LabTestRequestId",
                unique: true,
                filter: "[LabTestRequestId] IS NOT NULL");
        }
    }
}
