using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddLabTestRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LabTestRequestId",
                table: "WarehouseReceived",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "MoveOrders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "LabTestRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    Analysis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disposition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfSwab = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseReceived_LabTestRequestId",
                table: "WarehouseReceived",
                column: "LabTestRequestId",
                unique: true,
                filter: "[LabTestRequestId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseReceived_LabTestRequests_LabTestRequestId",
                table: "WarehouseReceived",
                column: "LabTestRequestId",
                principalTable: "LabTestRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseReceived_LabTestRequests_LabTestRequestId",
                table: "WarehouseReceived");

            migrationBuilder.DropTable(
                name: "LabTestRequests");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseReceived_LabTestRequestId",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "LabTestRequestId",
                table: "WarehouseReceived");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "MoveOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
